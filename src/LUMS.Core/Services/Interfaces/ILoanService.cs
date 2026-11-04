using LUMS.Core.Dtos;

namespace LUMS.Core.Services.Interfaces;

public interface ILoanService
{
    Task<IReadOnlyList<LoanDto>> GetActiveLoansAsync(int readerId, CancellationToken ct = default);

    Task<RenewLoanResult> RenewAsync(int loanId, int readerId, CancellationToken ct = default);
}
