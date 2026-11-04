using LUMS.Core.Dtos;
using LUMS.Core.Entities;

namespace LUMS.Core.Repositories;

public interface ILoanRepository : IRepository<Loan>
{
    Task<Loan?> GetByIdWithCopyAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<Loan>> GetActiveByReaderAsync(int readerId, CancellationToken ct = default);

    Task<int> CountActiveAsync(CancellationToken ct = default);

    Task<int> CountOverdueAsync(DateOnly today, CancellationToken ct = default);

    Task<IReadOnlyList<MonthlyLoanCount>> CountBorrowedPerMonthAsync(DateTime from, CancellationToken ct = default);
}
