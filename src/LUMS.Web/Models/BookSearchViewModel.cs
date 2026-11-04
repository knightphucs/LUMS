using LUMS.Core.Dtos;

namespace LUMS.Web.Models;

/// <summary>Dữ liệu cho màn hình OPAC (/Books) và Quản lý kho sách (/Staff/Books).</summary>
public class BookSearchViewModel
{
    public string? Keyword { get; init; }
    public string? Category { get; init; }
    public string? Author { get; init; }
    public string? Status { get; init; }

    public PagedResult<BookSummaryDto> Result { get; init; } = null!;

    public BookFilterOptionsDto Filters { get; init; } = null!;

    public PagerViewModel Pager { get; init; } = null!;

    public bool HasFilter =>
        !string.IsNullOrWhiteSpace(Keyword) || !string.IsNullOrWhiteSpace(Category) ||
        !string.IsNullOrWhiteSpace(Author) || !string.IsNullOrWhiteSpace(Status);

    /// <summary>Chuyển giá trị ô "Tình trạng" trên giao diện sang tiêu chí lọc của BookService.</summary>
    public static bool? ToInStock(string? status) => status switch
    {
        "instock" => true,
        "outofstock" => false,
        _ => null
    };
}
