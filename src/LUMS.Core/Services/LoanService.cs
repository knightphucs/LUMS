using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Exceptions;
using LUMS.Core.Interfaces.Common;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loans;
    private readonly IReservationRepository _reservations;
    private readonly IPolicyRepository _policies;
    private readonly ISystemLogRepository _logs;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public LoanService(
        ILoanRepository loans,
        IReservationRepository reservations,
        IPolicyRepository policies,
        ISystemLogRepository logs,
        IUnitOfWork uow,
        IClock clock)
    {
        _loans = loans;
        _reservations = reservations;
        _policies = policies;
        _logs = logs;
        _uow = uow;
        _clock = clock;
    }

    public async Task<IReadOnlyList<LoanDto>> GetActiveLoansAsync(int readerId, CancellationToken ct = default)
    {
        var today = _clock.Today;
        var loans = await _loans.GetActiveByReaderAsync(readerId, ct);
        var policy = await _policies.GetCurrentAsync(ct) ?? PolicyConfiguration.Default;

        var bookIds = loans.Select(l => l.Copy.BookTitleId).Distinct().ToList();
        var reserved = (await _reservations.GetBookIdsWithPendingReservationsAsync(bookIds, readerId, ct)).ToHashSet();

        return loans
            .Select(l =>
            {
                var overdueDays = l.GetOverdueDays(today);
                return new LoanDto(
                    l.Id,
                    l.CopyId,
                    l.Copy.BookTitle.Title,
                    DateOnly.FromDateTime(l.BorrowedAt),
                    l.DueDate,
                    l.RenewalCount,
                    overdueDays > 0,
                    overdueDays * policy.FinePerDay,
                    l.GetRenewBlockedReason(today, reserved.Contains(l.Copy.BookTitleId)));
            })
            .ToList();
    }

    public async Task<RenewLoanResult> RenewAsync(int loanId, int readerId, CancellationToken ct = default)
    {
        var loan = await _loans.GetByIdWithCopyAsync(loanId, ct)
                   ?? throw new NotFoundException("Không tìm thấy khoản mượn.");

        if (loan.ReaderId != readerId)
        {
            throw new ForbiddenException("Đây không phải khoản mượn của bạn.");
        }

        var policy = await _policies.GetCurrentAsync(ct) ?? PolicyConfiguration.Default;
        var pending = (await _reservations.GetBookIdsWithPendingReservationsAsync(
            new[] { loan.Copy.BookTitleId }, readerId, ct)).Count > 0;

        loan.Renew(_clock.Today, policy.MaxLoanDays, pending);

        await _logs.AddAsync(new SystemLog
        {
            AccountId = readerId,
            Action = $"Gia hạn sách {loan.CopyId} đến {loan.DueDate:dd/MM/yyyy}",
            RelatedObject = $"Loan:{loan.Id}",
            Timestamp = _clock.Now
        }, ct);

        await _uow.SaveChangesAsync(ct);

        return new RenewLoanResult(loan.Id, loan.DueDate, loan.RenewalCount);
    }
}
