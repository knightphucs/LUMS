using LUMS.Core.Dtos;

namespace LUMS.Core.Services.Interfaces;

public interface IBookService
{
    Task<PagedResult<BookSummaryDto>> SearchAsync(BookSearchCriteria criteria, CancellationToken ct = default);
    Task<BookDetailDto> GetDetailAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<BookSummaryDto>> GetLatestAsync(int take, CancellationToken ct = default);
    Task<BookFilterOptionsDto> GetFilterOptionsAsync(CancellationToken ct = default);
}
