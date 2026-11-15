using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Exceptions;
using LUMS.Core.Services;
using LUMS.Tests.Fakes;

namespace LUMS.Tests;

public class BookServiceTests
{
    private readonly FakeBookRepository _books = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeClock _clock = new(new DateTime(2026, 11, 6, 9, 0, 0));

    private BookService CreateSut() => new(_books, _uow, _clock);

    private static CreateBookRequest ValidRequest(Action<CreateBookRequest>? change = null)
    {
        var request = new CreateBookRequest
        {
            Title = "  Lập trình Web ASP.NET Core  ",
            Author = "Nguyễn Văn B",
            Category = "Công nghệ thông tin",
            Isbn = "978-604-01-9999-1",
            ShelfLocation = "C-03",
            CopyCount = 3
        };
        change?.Invoke(request);
        return request;
    }

    private BookTitle AddExistingBook(int id, string isbn, params string[] copyIds)
    {
        var book = new BookTitle { Id = id, Title = "Sách có sẵn", Isbn = isbn, DefaultShelf = "A-12" };
        foreach (var copyId in copyIds)
        {
            book.Copies.Add(new Copy { Id = copyId, BookTitleId = id, Shelf = "A-12", Status = CopyStatus.SanSang });
        }

        _books.Items.Add(book);
        return book;
    }

    // ---------- CreateBookAsync ----------

    [Fact]
    public async Task CreateBook_Valid_CreatesTitleWithAvailableCopiesAndSavesOnce()
    {
        var id = await CreateSut().CreateBookAsync(ValidRequest());

        var book = Assert.Single(_books.Items);
        Assert.Equal(id, book.Id);
        Assert.Equal("Lập trình Web ASP.NET Core", book.Title);
        Assert.Equal("978-604-01-9999-1", book.Isbn);
        Assert.Equal("C-03", book.DefaultShelf);
        Assert.Equal(_clock.Now, book.CreatedAt);

        Assert.Equal(3, book.Copies.Count);
        Assert.All(book.Copies, c =>
        {
            Assert.Equal(CopyStatus.SanSang, c.Status);
            Assert.Equal("C-03", c.Shelf);
            Assert.Equal(_clock.Now, c.AddedAt);
        });
        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task CreateBook_GeneratesCopyIdsFromIsbnDigits()
    {
        await CreateSut().CreateBookAsync(ValidRequest());

        var ids = _books.Items.Single().Copies.Select(c => c.Id).ToList();
        Assert.Equal(new[]
        {
            "LUMS-9786040199991-001",
            "LUMS-9786040199991-002",
            "LUMS-9786040199991-003"
        }, ids);
    }

    [Fact]
    public async Task CreateBook_DuplicateIsbn_ThrowsWithExistingIdAndDoesNotSave()
    {
        AddExistingBook(id: 7, isbn: "978-604-01-9999-1");

        // Cùng ISBN nhưng viết khác dấu gạch ngang vẫn bị coi là trùng.
        var ex = await Assert.ThrowsAsync<DuplicateIsbnException>(
            () => CreateSut().CreateBookAsync(ValidRequest(r => r.Isbn = "9786040199991")));

        Assert.Equal(7, ex.ExistingBookId);
        Assert.Contains("#7", ex.Message);
        Assert.Single(_books.Items);
        Assert.Equal(0, _uow.SaveCount);
    }

    [Theory]
    [InlineData("", "Nguyễn Văn B", "CNTT", "C-03")]
    [InlineData("Tên sách", "   ", "CNTT", "C-03")]
    [InlineData("Tên sách", "Nguyễn Văn B", "", "C-03")]
    [InlineData("Tên sách", "Nguyễn Văn B", "CNTT", "")]
    public async Task CreateBook_MissingRequiredField_ThrowsDomainException(
        string title, string author, string category, string shelf)
    {
        var request = ValidRequest(r =>
        {
            r.Title = title;
            r.Author = author;
            r.Category = category;
            r.ShelfLocation = shelf;
        });

        await Assert.ThrowsAsync<DomainException>(() => CreateSut().CreateBookAsync(request));

        Assert.Empty(_books.Items);
        Assert.Equal(0, _uow.SaveCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("978-604-ABC-123")]
    [InlineData("97860401999991234")]
    public async Task CreateBook_InvalidIsbn_ThrowsDomainException(string isbn)
    {
        await Assert.ThrowsAsync<DomainException>(
            () => CreateSut().CreateBookAsync(ValidRequest(r => r.Isbn = isbn)));

        Assert.Equal(0, _uow.SaveCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(CreateBookRequest.MaxCopyCount + 1)]
    public async Task CreateBook_InvalidCopyCount_ThrowsDomainException(int copyCount)
    {
        await Assert.ThrowsAsync<DomainException>(
            () => CreateSut().CreateBookAsync(ValidRequest(r => r.CopyCount = copyCount)));

        Assert.Equal(0, _uow.SaveCount);
    }

    [Fact]
    public async Task CreateBook_Isbn10WithCheckDigitX_IsAccepted()
    {
        await CreateSut().CreateBookAsync(ValidRequest(r => { r.Isbn = "0-306-40615-x"; r.CopyCount = 1; }));

        Assert.Equal("LUMS-030640615X-001", _books.Items.Single().Copies.Single().Id);
    }

    // ---------- AddCopiesAsync ----------

    [Fact]
    public async Task AddCopies_ContinuesFromHighestExistingSequence()
    {
        var book = AddExistingBook(1, "978-604-01-2345",
            "LUMS-CS101-001", "LUMS-CS101-002", "LUMS-CS101-005");

        var created = await CreateSut().AddCopiesAsync(1, 2);

        Assert.Equal(new[] { "LUMS-CS101-006", "LUMS-CS101-007" }, created);
        Assert.Equal(5, book.Copies.Count);
        var added = book.Copies.Where(c => created.Contains(c.Id)).ToList();
        Assert.All(added, c =>
        {
            Assert.Equal(CopyStatus.SanSang, c.Status);
            Assert.Equal("A-12", c.Shelf);
            Assert.Equal(_clock.Now, c.AddedAt);
        });
        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task AddCopies_WhenBookHasNoCopies_StartsAt001WithIsbnCode()
    {
        AddExistingBook(1, "978-604-01-2345");

        var created = await CreateSut().AddCopiesAsync(1, 1);

        Assert.Equal(new[] { "LUMS-978604012345-001" }, created);
    }

    [Fact]
    public async Task AddCopies_AfterCreateBook_ContinuesSequence()
    {
        var sut = CreateSut();
        var id = await sut.CreateBookAsync(ValidRequest(r => r.CopyCount = 5));

        var created = await sut.AddCopiesAsync(id, 1);

        Assert.Equal(new[] { "LUMS-9786040199991-006" }, created);
        Assert.Equal(2, _uow.SaveCount);
    }

    [Fact]
    public async Task AddCopies_BookMissing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => CreateSut().AddCopiesAsync(404, 1));

        Assert.Equal(0, _uow.SaveCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(CreateBookRequest.MaxCopyCount + 1)]
    public async Task AddCopies_InvalidCount_ThrowsDomainException(int count)
    {
        AddExistingBook(1, "978-604-01-2345", "LUMS-CS101-001");

        await Assert.ThrowsAsync<DomainException>(() => CreateSut().AddCopiesAsync(1, count));

        Assert.Equal(0, _uow.SaveCount);
    }

    [Fact]
    public async Task AddCopies_ExceedingMaxSequence_ThrowsDomainException()
    {
        AddExistingBook(1, "978-604-01-2345", "LUMS-CS101-998");

        await Assert.ThrowsAsync<DomainException>(() => CreateSut().AddCopiesAsync(1, 2));

        Assert.Equal(0, _uow.SaveCount);
    }

    // ---------- UpdateBookAsync ----------

    private static UpdateBookRequest ValidUpdate(Action<UpdateBookRequest>? change = null)
    {
        var request = new UpdateBookRequest
        {
            Title = "  Lập trình C# Nâng cao  ",
            Author = "Trần Văn K",
            Category = "Công nghệ thông tin",
            Isbn = "978-604-01-2345",
            ShelfLocation = "D-07"
        };
        change?.Invoke(request);
        return request;
    }

    [Fact]
    public async Task UpdateBook_Valid_UpdatesFieldsMovesDefaultShelfCopiesAndSavesOnce()
    {
        var book = AddExistingBook(1, "978-604-01-2345", "LUMS-CS101-001", "LUMS-CS101-002");
        book.Copies.First(c => c.Id == "LUMS-CS101-002").Shelf = "KHO-01"; // bản sao đã xếp kệ riêng

        await CreateSut().UpdateBookAsync(1, ValidUpdate(r => r.Isbn = "978-604-01-7777"));

        Assert.Equal("Lập trình C# Nâng cao", book.Title);
        Assert.Equal("Trần Văn K", book.Author);
        Assert.Equal("978-604-01-7777", book.Isbn);
        Assert.Equal("D-07", book.DefaultShelf);
        Assert.Equal("D-07", book.Copies.Single(c => c.Id == "LUMS-CS101-001").Shelf);
        Assert.Equal("KHO-01", book.Copies.Single(c => c.Id == "LUMS-CS101-002").Shelf);
        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task UpdateBook_SameIsbnWrittenDifferently_IsNotTreatedAsDuplicate()
    {
        AddExistingBook(1, "978-604-01-2345");

        await CreateSut().UpdateBookAsync(1, ValidUpdate(r => r.Isbn = "978604012345"));

        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task UpdateBook_IsbnOfAnotherBook_ThrowsDuplicateIsbnAndDoesNotChange()
    {
        var book = AddExistingBook(1, "978-604-01-2345");
        AddExistingBook(2, "978-604-01-2346");

        var ex = await Assert.ThrowsAsync<DuplicateIsbnException>(
            () => CreateSut().UpdateBookAsync(1, ValidUpdate(r => r.Isbn = "9786040123-46")));

        Assert.Equal(2, ex.ExistingBookId);
        Assert.Equal("978-604-01-2345", book.Isbn);
        Assert.Equal("Sách có sẵn", book.Title);
        Assert.Equal(0, _uow.SaveCount);
    }

    [Theory]
    [InlineData("", "Trần Văn K", "978-604-01-2345")]
    [InlineData("Tên sách", "  ", "978-604-01-2345")]
    [InlineData("Tên sách", "Trần Văn K", "12345")]
    public async Task UpdateBook_InvalidInput_ThrowsDomainException(string title, string author, string isbn)
    {
        var book = AddExistingBook(1, "978-604-01-2345");

        await Assert.ThrowsAsync<DomainException>(() => CreateSut().UpdateBookAsync(1, ValidUpdate(r =>
        {
            r.Title = title;
            r.Author = author;
            r.Isbn = isbn;
        })));

        Assert.Equal("Sách có sẵn", book.Title);
        Assert.Equal(0, _uow.SaveCount);
    }

    [Fact]
    public async Task UpdateBook_BookMissing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => CreateSut().UpdateBookAsync(404, ValidUpdate()));

        Assert.Equal(0, _uow.SaveCount);
    }

    // ---------- GetMergeSuggestionAsync ----------

    [Fact]
    public async Task GetMergeSuggestion_ExistingIsbn_ReturnsBookTitleAndCopyCount()
    {
        AddExistingBook(7, "978-604-01-2345", "LUMS-CS101-001", "LUMS-CS101-002", "LUMS-CS101-003");

        var suggestion = await CreateSut().GetMergeSuggestionAsync("978604012345");

        Assert.NotNull(suggestion);
        Assert.Equal(new MergeSuggestionDto(7, "Sách có sẵn", "978-604-01-2345", 3), suggestion);
        Assert.Equal(0, _uow.SaveCount);
    }

    [Fact]
    public async Task GetMergeSuggestion_UnknownIsbn_ReturnsNull()
    {
        AddExistingBook(7, "978-604-01-2345");

        Assert.Null(await CreateSut().GetMergeSuggestionAsync("978-604-01-9999-1"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("978-604-ABC-123")]
    public async Task GetMergeSuggestion_InvalidIsbn_ThrowsDomainException(string isbn)
    {
        await Assert.ThrowsAsync<DomainException>(() => CreateSut().GetMergeSuggestionAsync(isbn));
    }

    // ---------- SearchAsync (FR-01-01) ----------
    // Lưu ý: phần lọc thật (LIKE theo tên/tác giả/ISBN, thể loại, còn/hết) chạy trong EF Core ở
    // BookRepository nên không kiểm được bằng FakeBookRepository. Các test dưới chỉ kiểm phần logic
    // của BookService: chuẩn hóa tham số, mức tình trạng, tổng trang, kết quả rỗng.

    private void AddBooksWithAvailable(int count, int availableCopies = 2)
    {
        for (var i = 1; i <= count; i++)
        {
            var book = new BookTitle { Id = i, Title = $"Sách {i:000}", Author = "Tác giả", Category = "CNTT", Isbn = $"978604{i:000000}" };
            for (var c = 1; c <= availableCopies; c++)
            {
                book.Copies.Add(new Copy { Id = $"LUMS-T{i}-{c:000}", Status = CopyStatus.SanSang });
            }

            _books.Items.Add(book);
        }
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(3, 3)]
    public async Task Search_PageBelowOne_IsNormalizedToFirstPage(int page, int expected)
    {
        AddBooksWithAvailable(30);

        var result = await CreateSut().SearchAsync(new BookSearchCriteria { Page = page, PageSize = 10 });

        Assert.Equal(expected, result.Page);
        Assert.Equal(10, result.Items.Count);
    }

    [Fact]
    public async Task Search_PageSizeAboveMax_IsCappedAtMaxPageSize()
    {
        AddBooksWithAvailable(60);

        var result = await CreateSut().SearchAsync(new BookSearchCriteria { PageSize = 500 });

        Assert.Equal(BookSearchCriteria.MaxPageSize, result.PageSize);
        Assert.Equal(50, result.Items.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Search_PageSizeBelowOne_UsesDefaultPageSize(int pageSize)
    {
        AddBooksWithAvailable(20);
        var defaultPageSize = new BookSearchCriteria().PageSize;

        var result = await CreateSut().SearchAsync(new BookSearchCriteria { PageSize = pageSize });

        Assert.Equal(defaultPageSize, result.PageSize);
        Assert.Equal(defaultPageSize, result.Items.Count);
    }

    [Fact]
    public async Task Search_WhitespaceFilters_AreNormalizedToNullAndTrimmed()
    {
        AddBooksWithAvailable(3);
        var criteria = new BookSearchCriteria { Keyword = "   ", Category = "\t", Author = "  Tác giả  " };

        var result = await CreateSut().SearchAsync(criteria);

        Assert.Null(criteria.Keyword);
        Assert.Null(criteria.Category);
        Assert.Equal("Tác giả", criteria.Author);
        Assert.Equal(3, result.TotalCount); // khoảng trắng không biến thành điều kiện lọc
    }

    [Theory]
    [InlineData(0, AvailabilityLevel.OutOfStock)]
    [InlineData(1, AvailabilityLevel.Low)]
    [InlineData(2, AvailabilityLevel.Available)]
    [InlineData(5, AvailabilityLevel.Available)]
    public async Task Search_AvailableCopies_MapToAvailabilityLevel(int available, AvailabilityLevel expected)
    {
        var book = AddExistingBook(1, "978-604-01-2345");
        for (var i = 1; i <= available; i++)
        {
            book.Copies.Add(new Copy { Id = $"LUMS-CS101-{i:000}", Status = CopyStatus.SanSang });
        }
        book.Copies.Add(new Copy { Id = "LUMS-CS101-900", Status = CopyStatus.DangDuocMuon }); // bản đang mượn không tính

        var item = Assert.Single((await CreateSut().SearchAsync(new BookSearchCriteria())).Items);

        Assert.Equal(expected, item.Availability);
        Assert.Equal(available, item.AvailableCopies);
        Assert.Equal(available + 1, item.TotalCopies);
    }

    [Theory]
    [InlineData(21, 10, 3)]
    [InlineData(20, 10, 2)]
    [InlineData(1, 8, 1)]
    public async Task Search_TotalPages_IsCeilingOfTotalOverPageSize(int total, int pageSize, int expectedPages)
    {
        AddBooksWithAvailable(total);

        var result = await CreateSut().SearchAsync(new BookSearchCriteria { PageSize = pageSize });

        Assert.Equal(total, result.TotalCount);
        Assert.Equal(expectedPages, result.TotalPages);
    }

    [Fact]
    public async Task Search_NoMatch_ReturnsEmptyListAndZeroPages()
    {
        AddBooksWithAvailable(5);

        var result = await CreateSut().SearchAsync(new BookSearchCriteria { Keyword = "không-có-sách-này" });

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }
}
