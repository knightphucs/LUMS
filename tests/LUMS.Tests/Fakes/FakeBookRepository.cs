using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Interfaces.Repositories;

namespace LUMS.Tests.Fakes;

/// <summary>
/// Kho sách trong bộ nhớ cho unit test BookService (theo mẫu FakeLoanRepository).
/// AddAsync tự cấp Id tăng dần để mô phỏng cột identity của SQL Server.
/// </summary>
public sealed class FakeBookRepository : IBookRepository
{
    public List<BookTitle> Items { get; } = new();

    public Task<BookTitle?> GetByIdAsync(object id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(b => b.Id.Equals(id)));

    public Task<(IReadOnlyList<BookTitle> Items, int TotalCount)> SearchAsync(
        BookSearchCriteria criteria, CancellationToken ct = default)
    {
        IEnumerable<BookTitle> query = Items;

        if (criteria.Keyword is not null)
            query = query.Where(b => b.Title.Contains(criteria.Keyword) || b.Author.Contains(criteria.Keyword) || b.Isbn.Contains(criteria.Keyword));
        if (criteria.Category is not null)
            query = query.Where(b => b.Category == criteria.Category);
        if (criteria.Author is not null)
            query = query.Where(b => b.Author == criteria.Author);
        if (criteria.InStock is bool inStock)
            query = query.Where(b => b.Copies.Any(c => c.Status == CopyStatus.SanSang) == inStock);

        var all = query.OrderBy(b => b.Title).ThenBy(b => b.Id).ToList();
        var page = all.Skip((criteria.Page - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();
        return Task.FromResult<(IReadOnlyList<BookTitle>, int)>((page, all.Count));
    }

    public Task<BookTitle?> GetWithCopiesAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(b => b.Id == id));

    public Task<BookTitle?> GetWithCopiesForUpdateAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(b => b.Id == id));

    public Task<IReadOnlyList<BookTitle>> GetLatestAsync(int take, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<BookTitle>>(
            Items.OrderByDescending(b => b.CreatedAt).ThenByDescending(b => b.Id).Take(take).ToList());

    public Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<string>>(Items.Select(b => b.Category).Distinct().OrderBy(x => x).ToList());

    public Task<IReadOnlyList<string>> GetAuthorsAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<string>>(Items.Select(b => b.Author).Distinct().OrderBy(x => x).ToList());

    public Task<BookTitle?> GetByIsbnAsync(string isbn, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(b => b.Isbn.Replace("-", "").Replace(" ", "") == isbn));

    public Task AddAsync(BookTitle entity, CancellationToken ct = default)
    {
        if (entity.Id == 0)
        {
            entity.Id = Items.Count == 0 ? 1 : Items.Max(b => b.Id) + 1;
        }

        foreach (var copy in entity.Copies)
        {
            copy.BookTitleId = entity.Id;
            copy.BookTitle = entity;
        }

        Items.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(BookTitle entity)
    {
    }

    public void Remove(BookTitle entity) => Items.Remove(entity);
}
