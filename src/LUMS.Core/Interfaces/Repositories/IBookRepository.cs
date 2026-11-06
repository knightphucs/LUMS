using LUMS.Core.Dtos;
using LUMS.Core.Entities;

namespace LUMS.Core.Interfaces.Repositories;

public interface IBookRepository : IRepository<BookTitle>
{
    Task<(IReadOnlyList<BookTitle> Items, int TotalCount)> SearchAsync(BookSearchCriteria criteria, CancellationToken ct = default);

    Task<BookTitle?> GetWithCopiesAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<BookTitle>> GetLatestAsync(int take, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetAuthorsAsync(CancellationToken ct = default);

    /// <summary>
    /// Tìm đầu sách theo ISBN, so sánh sau khi bỏ dấu gạch ngang và khoảng trắng
    /// (vd "978-604-01-2345" khớp "978604012345"). Truyền vào ISBN đã chuẩn hoá.
    /// </summary>
    Task<BookTitle?> GetByIsbnAsync(string isbn, CancellationToken ct = default);

    /// <summary>Lấy đầu sách kèm bản sao, CÓ tracking — dùng khi cần thêm/sửa bản sao rồi SaveChanges.</summary>
    Task<BookTitle?> GetWithCopiesForUpdateAsync(int id, CancellationToken ct = default);
}
