using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Exceptions;
using LUMS.Core.Interfaces.Common;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class BookService : IBookService
{
    public const int LowStockThreshold = 1;

    /// <summary>Số thứ tự bản sao định dạng {i:000} nên tối đa 999 bản sao mỗi đầu sách.</summary>
    public const int MaxCopySequence = 999;

    private const string CopyIdPrefix = "LUMS";

    private readonly IBookRepository _books;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public BookService(IBookRepository books, IUnitOfWork uow, IClock clock)
    {
        _books = books;
        _uow = uow;
        _clock = clock;
    }

    public async Task<PagedResult<BookSummaryDto>> SearchAsync(BookSearchCriteria criteria, CancellationToken ct = default)
    {
        criteria.Normalize();
        var (items, total) = await _books.SearchAsync(criteria, ct);
        return new PagedResult<BookSummaryDto>(items.Select(ToSummary).ToList(), total, criteria.Page, criteria.PageSize);
    }

    public async Task<BookDetailDto> GetDetailAsync(int id, CancellationToken ct = default)
    {
        var book = await _books.GetWithCopiesAsync(id, ct)
                   ?? throw new NotFoundException("Không tìm thấy đầu sách (có thể đã bị gỡ khỏi kho).");

        var copies = book.Copies
            .OrderBy(c => c.Id)
            .Select(c => new CopyDto(c.Id, c.Shelf, c.Status))
            .ToList();
        var available = CountAvailable(book);

        return new BookDetailDto(
            book.Id, book.Title, book.Author, book.Category, book.Isbn, book.Description, book.CoverImage,
            book.DefaultShelf, available, book.Copies.Count, LevelOf(available), copies);
    }

    public async Task<IReadOnlyList<BookSummaryDto>> GetLatestAsync(int take, CancellationToken ct = default)
    {
        var items = await _books.GetLatestAsync(Math.Clamp(take, 1, 20), ct);
        return items.Select(ToSummary).ToList();
    }

    public async Task<BookFilterOptionsDto> GetFilterOptionsAsync(CancellationToken ct = default)
    {
        var categories = await _books.GetCategoriesAsync(ct);
        var authors = await _books.GetAuthorsAsync(ct);
        return new BookFilterOptionsDto(categories, authors);
    }

    public async Task<int> CreateBookAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var title = Required(request.Title, "Tên sách", 200);
        var author = Required(request.Author, "Tác giả", 150);
        var category = Required(request.Category, "Thể loại", 100);
        var shelf = Required(request.ShelfLocation, "Vị trí kệ", 20);
        var isbn = Required(request.Isbn, "Mã ISBN", 20);
        var isbnCode = NormalizeIsbn(isbn);
        ValidateCopyCount(request.CopyCount);

        var existing = await _books.GetByIsbnAsync(isbnCode, ct);
        if (existing is not null)
        {
            throw new DuplicateIsbnException(isbn, existing.Id);
        }

        var now = _clock.Now;
        var book = new BookTitle
        {
            Title = title,
            Author = author,
            Category = category,
            Isbn = isbn,
            DefaultShelf = shelf,
            CreatedAt = now
        };

        var prefix = $"{CopyIdPrefix}-{isbnCode}";
        for (var i = 1; i <= request.CopyCount; i++)
        {
            book.Copies.Add(NewCopy(prefix, i, shelf, now));
        }

        await _books.AddAsync(book, ct);
        await _uow.SaveChangesAsync(ct);
        return book.Id;
    }

    public async Task<IReadOnlyList<string>> AddCopiesAsync(int bookId, int count, CancellationToken ct = default)
    {
        ValidateCopyCount(count);

        var book = await _books.GetWithCopiesForUpdateAsync(bookId, ct)
                   ?? throw new NotFoundException("Không tìm thấy đầu sách (có thể đã bị gỡ khỏi kho).");

        // Nối tiếp theo bản sao có số thứ tự lớn nhất; giữ nguyên tiền tố của nó
        // (sách seed dùng mã kiểu LUMS-CS101-xxx, sách mới dùng LUMS-{ISBN}-xxx).
        var last = book.Copies
            .Select(c => ParseCopyId(c.Id))
            .Where(p => p is not null)
            .Select(p => p!.Value)
            .OrderByDescending(p => p.Sequence)
            .FirstOrDefault();

        var prefix = last.Prefix ?? $"{CopyIdPrefix}-{NormalizeIsbn(book.Isbn)}";
        var start = last.Sequence + 1;
        if (start + count - 1 > MaxCopySequence)
        {
            throw new DomainException(
                $"Đầu sách đã có bản sao số {last.Sequence}; chỉ thêm được tối đa {MaxCopySequence - last.Sequence} bản sao nữa.");
        }

        var now = _clock.Now;
        var created = new List<string>(count);
        for (var i = start; i < start + count; i++)
        {
            var copy = NewCopy(prefix, i, book.DefaultShelf, now);
            book.Copies.Add(copy);
            created.Add(copy.Id);
        }

        await _uow.SaveChangesAsync(ct);
        return created;
    }

    /// <summary>
    /// Bỏ dấu gạch ngang/khoảng trắng, viết hoa; còn lại 10–13 ký tự số (ký tự cuối được là X với ISBN-10).
    /// Kết quả dùng làm phần {mã} trong mã bản sao LUMS-{mã}-{i:000}.
    /// </summary>
    public static string NormalizeIsbn(string isbn)
    {
        var code = new string(isbn.Where(ch => ch != '-' && !char.IsWhiteSpace(ch)).ToArray()).ToUpperInvariant();

        var valid = code.Length is >= 10 and <= 13
                    && code[..^1].All(char.IsAsciiDigit)
                    && (char.IsAsciiDigit(code[^1]) || code[^1] == 'X');
        if (!valid)
        {
            throw new DomainException("Mã ISBN không hợp lệ: chỉ gồm 10–13 chữ số (có thể có dấu gạch ngang).");
        }

        return code;
    }

    private static Copy NewCopy(string prefix, int sequence, string shelf, DateTime now) => new()
    {
        Id = $"{prefix}-{sequence:000}",
        Shelf = shelf,
        Status = CopyStatus.SanSang,
        AddedAt = now
    };

    private static (string? Prefix, int Sequence)? ParseCopyId(string id)
    {
        var dash = id.LastIndexOf('-');
        if (dash <= 0 || !int.TryParse(id.AsSpan(dash + 1), out var sequence))
        {
            return null;
        }

        return (id[..dash], sequence);
    }

    private static void ValidateCopyCount(int count)
    {
        if (count < 1 || count > CreateBookRequest.MaxCopyCount)
        {
            throw new DomainException($"Số lượng bản sao phải từ 1 đến {CreateBookRequest.MaxCopyCount}.");
        }
    }

    private static string Required(string? value, string field, int maxLength)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            throw new DomainException($"{field} không được để trống.");
        }

        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"{field} tối đa {maxLength} ký tự.");
        }

        return trimmed;
    }

    private static BookSummaryDto ToSummary(BookTitle b)
    {
        var available = CountAvailable(b);
        return new BookSummaryDto(
            b.Id, b.Title, b.Author, b.Category, b.DefaultShelf, b.CoverImage,
            available, b.Copies.Count, LevelOf(available));
    }

    private static int CountAvailable(BookTitle b) => b.Copies.Count(c => c.Status == CopyStatus.SanSang);

    private static AvailabilityLevel LevelOf(int available) =>
        available <= 0 ? AvailabilityLevel.OutOfStock
        : available <= LowStockThreshold ? AvailabilityLevel.Low
        : AvailabilityLevel.Available;
}
