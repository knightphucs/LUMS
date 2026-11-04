using LUMS.Core.Dtos;

namespace LUMS.Web.Models;

public class HomeIndexViewModel
{
    public IReadOnlyList<BookSummaryDto> LatestBooks { get; init; } = Array.Empty<BookSummaryDto>();
}
