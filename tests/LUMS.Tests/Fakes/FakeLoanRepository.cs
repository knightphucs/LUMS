using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Interfaces.Repositories;

namespace LUMS.Tests.Fakes;

public sealed class FakeLoanRepository : ILoanRepository
{
    public List<Loan> Items { get; } = new();

    public Task<Loan?> GetByIdAsync(object id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(l => l.Id.Equals(id)));

    public Task<Loan?> GetByIdWithCopyAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(l => l.Id == id));

    public Task<IReadOnlyList<Loan>> GetActiveByReaderAsync(int readerId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Loan>>(
            Items.Where(l => l.ReaderId == readerId && l.ReturnedDate is null).ToList());

    public Task<int> CountActiveAsync(CancellationToken ct = default) =>
        Task.FromResult(Items.Count(l => l.ReturnedDate is null));

    public Task<int> CountOverdueAsync(DateOnly today, CancellationToken ct = default) =>
        Task.FromResult(Items.Count(l => l.ReturnedDate is null && l.DueDate < today));

    public Task<IReadOnlyList<MonthlyLoanCount>> CountBorrowedPerMonthAsync(DateTime from, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<MonthlyLoanCount>>(
            Items.Where(l => l.BorrowedAt >= from)
                 .GroupBy(l => new { l.BorrowedAt.Year, l.BorrowedAt.Month })
                 .Select(g => new MonthlyLoanCount(g.Key.Year, g.Key.Month, g.Count()))
                 .ToList());

    public Task AddAsync(Loan entity, CancellationToken ct = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Loan entity)
    {
    }

    public void Remove(Loan entity) => Items.Remove(entity);
}
