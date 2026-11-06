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
}
