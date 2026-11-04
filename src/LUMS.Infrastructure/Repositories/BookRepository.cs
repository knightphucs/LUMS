using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Repositories;

public class BookRepository : EfRepository<BookTitle>, IBookRepository
{
    public BookRepository(LumsDbContext db) : base(db)
    {
    }

    public async Task<(IReadOnlyList<BookTitle> Items, int TotalCount)> SearchAsync(
        BookSearchCriteria criteria, CancellationToken ct = default)
    {
        IQueryable<BookTitle> query = Db.BookTitles.AsNoTracking();

        if (criteria.Keyword is not null)
        {
            var keyword = criteria.Keyword;
            query = query.Where(b => b.Title.Contains(keyword) || b.Author.Contains(keyword) || b.Isbn.Contains(keyword));
        }

        if (criteria.Category is not null)
        {
            var category = criteria.Category;
            query = query.Where(b => b.Category == category);
        }

        if (criteria.Author is not null)
        {
            var author = criteria.Author;
            query = query.Where(b => b.Author == author);
        }

        if (criteria.InStock == true)
        {
            query = query.Where(b => b.Copies.Any(c => c.Status == CopyStatus.SanSang));
        }
        else if (criteria.InStock == false)
        {
            query = query.Where(b => !b.Copies.Any(c => c.Status == CopyStatus.SanSang));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .Include(b => b.Copies)
            .OrderBy(b => b.Title).ThenBy(b => b.Id)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<BookTitle?> GetWithCopiesAsync(int id, CancellationToken ct = default) =>
        Db.BookTitles.AsNoTracking()
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IReadOnlyList<BookTitle>> GetLatestAsync(int take, CancellationToken ct = default) =>
        await Db.BookTitles.AsNoTracking()
            .Include(b => b.Copies)
            .OrderByDescending(b => b.CreatedAt).ThenByDescending(b => b.Id)
            .Take(take)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default) =>
        await Db.BookTitles.AsNoTracking().Select(b => b.Category).Distinct().OrderBy(x => x).ToListAsync(ct);

    public async Task<IReadOnlyList<string>> GetAuthorsAsync(CancellationToken ct = default) =>
        await Db.BookTitles.AsNoTracking().Select(b => b.Author).Distinct().OrderBy(x => x).ToListAsync(ct);
}
