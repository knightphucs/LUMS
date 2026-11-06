using LUMS.Core.Enums;

namespace LUMS.Core.Dtos;

public enum AvailabilityLevel
{
    Available,
    Low,
    OutOfStock
}

public class BookSearchCriteria
{
    public const int MaxPageSize = 50;

    public string? Keyword { get; set; }
    public string? Category { get; set; }
    public string? Author { get; set; }

    public bool? InStock { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 8;

    public void Normalize()
    {
        Keyword = string.IsNullOrWhiteSpace(Keyword) ? null : Keyword.Trim();
        Category = string.IsNullOrWhiteSpace(Category) ? null : Category.Trim();
        Author = string.IsNullOrWhiteSpace(Author) ? null : Author.Trim();
        if (Page < 1) Page = 1;
        if (PageSize < 1) PageSize = 8;
        if (PageSize > MaxPageSize) PageSize = MaxPageSize;
    }
}

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public record BookSummaryDto(
    int Id,
    string Title,
    string Author,
    string Category,
    string Shelf,
    string? CoverImage,
    int AvailableCopies,
    int TotalCopies,
    AvailabilityLevel Availability);

public record CopyDto(string Id, string Shelf, CopyStatus Status);

public record BookDetailDto(
    int Id,
    string Title,
    string Author,
    string Category,
    string Isbn,
    string? Description,
    string? CoverImage,
    string Shelf,
    int AvailableCopies,
    int TotalCopies,
    AvailabilityLevel Availability,
    IReadOnlyList<CopyDto> Copies);

public record BookFilterOptionsDto(IReadOnlyList<string> Categories, IReadOnlyList<string> Authors);

/// <summary>
/// Dữ liệu thủ thư nhập khi thêm đầu sách mới (FR-02-03).
/// ShelfLocation được gán cho cả BookTitle.DefaultShelf lẫn Copy.Shelf của từng bản sao.
/// Là class (không phải positional record) để MVC model binding đọc được từ form.
/// </summary>
public class CreateBookRequest
{
    public const int MaxCopyCount = 100;

    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string ShelfLocation { get; set; } = string.Empty;
    public int CopyCount { get; set; }
}
