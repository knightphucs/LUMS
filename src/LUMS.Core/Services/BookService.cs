using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Exceptions;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class BookService : IBookService
{
    public const int LowStockThreshold = 1;

    private readonly IBookRepository _books;

    public BookService(IBookRepository books)
    {
        _books = books;
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
