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
}
