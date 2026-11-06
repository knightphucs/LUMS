using LUMS.Core.Dtos;

namespace LUMS.Core.Interfaces.Services;

public interface IBookService
{
    Task<PagedResult<BookSummaryDto>> SearchAsync(BookSearchCriteria criteria, CancellationToken ct = default);
    Task<BookDetailDto> GetDetailAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<BookSummaryDto>> GetLatestAsync(int take, CancellationToken ct = default);
    Task<BookFilterOptionsDto> GetFilterOptionsAsync(CancellationToken ct = default);

    /// <summary>
    /// Thêm đầu sách mới cùng CopyCount bản sao trạng thái SanSang (FR-02-03).
    /// Sai đầu vào → DomainException; trùng ISBN → DuplicateIsbnException (kèm Id đầu sách cũ).
    /// </summary>
    /// <returns>Id đầu sách vừa tạo.</returns>
    Task<int> CreateBookAsync(CreateBookRequest request, CancellationToken ct = default);

    /// <summary>
    /// Thêm count bản sao cho đầu sách có sẵn; số thứ tự nối tiếp số lớn nhất hiện có
    /// (đang có …-005 thì sinh …-006).
    /// </summary>
    /// <returns>Mã các bản sao vừa tạo.</returns>
    Task<IReadOnlyList<string>> AddCopiesAsync(int bookId, int count, CancellationToken ct = default);
}
