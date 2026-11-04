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
