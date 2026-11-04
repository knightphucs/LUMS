using LUMS.Core.Dtos;

namespace LUMS.Web.Models;

/// <summary>Dữ liệu cho View Component BookFilters.</summary>
public class BookFiltersViewModel
{
    public BookFilterOptionsDto Options { get; init; } = null!;

    public string? Category { get; init; }
    public string? Author { get; init; }
    public string? Status { get; init; }

    /// <summary>Giao diện khu vực nhân viên: ô vuông, không có bộ lọc Tác giả.</summary>
    public bool Staff { get; init; }
}
