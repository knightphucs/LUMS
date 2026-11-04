using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Repositories;
using LUMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Repositories;

public class LoanRepository : EfRepository<Loan>, ILoanRepository
{
    public LoanRepository(LumsDbContext db) : base(db)
    {
    }

    public Task<Loan?> GetByIdWithCopyAsync(int id, CancellationToken ct = default) =>
        Db.Loans.Include(l => l.Copy).FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<IReadOnlyList<Loan>> GetActiveByReaderAsync(int readerId, CancellationToken ct = default) =>
        await Db.Loans.AsNoTracking()
            .Include(l => l.Copy).ThenInclude(c => c.BookTitle)
            .Where(l => l.ReaderId == readerId && l.Status != LoanStatus.DaTra)
            .OrderBy(l => l.DueDate).ThenBy(l => l.Id)
            .ToListAsync(ct);

    public Task<int> CountActiveAsync(CancellationToken ct = default) =>
        Db.Loans.CountAsync(l => l.Status != LoanStatus.DaTra, ct);

    public Task<int> CountOverdueAsync(DateOnly today, CancellationToken ct = default) =>
        Db.Loans.CountAsync(
            l => l.Status == LoanStatus.QuaHan || (l.Status == LoanStatus.DangMuon && l.DueDate < today), ct);

    public async Task<IReadOnlyList<MonthlyLoanCount>> CountBorrowedPerMonthAsync(DateTime from, CancellationToken ct = default) =>
        await Db.Loans.AsNoTracking()
            .Where(l => l.BorrowedAt >= from)
            .GroupBy(l => new { l.BorrowedAt.Year, l.BorrowedAt.Month })
            .Select(g => new MonthlyLoanCount(g.Key.Year, g.Key.Month, g.Count()))
            .ToListAsync(ct);
}
