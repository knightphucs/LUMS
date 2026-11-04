using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Interfaces.Common;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class DashboardService : IDashboardService
{
    private const int ChartMonths = 8;

    private readonly ILoanRepository _loans;
    private readonly IFineRepository _fines;
    private readonly IAccountRepository _accounts;
    private readonly ISystemLogRepository _logs;
    private readonly IPolicyRepository _policies;
    private readonly IClock _clock;

    public DashboardService(
        ILoanRepository loans,
        IFineRepository fines,
        IAccountRepository accounts,
        ISystemLogRepository logs,
        IPolicyRepository policies,
        IClock clock)
    {
        _loans = loans;
        _fines = fines;
        _accounts = accounts;
        _logs = logs;
        _policies = policies;
        _clock = clock;
    }

    public async Task<AdminDashboardDto> GetAsync(CancellationToken ct = default)
    {
        var now = _clock.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var active = await _loans.CountActiveAsync(ct);
        var overdue = await _loans.CountOverdueAsync(_clock.Today, ct);
        var fines = await _fines.SumCollectedAsync(monthStart, monthStart.AddMonths(1), ct);
        var accounts = await _accounts.CountAsync(ct);

        var from = monthStart.AddMonths(-(ChartMonths - 1));
        var raw = await _loans.CountBorrowedPerMonthAsync(from, ct);
        var monthly = Enumerable.Range(0, ChartMonths)
            .Select(i => from.AddMonths(i))
            .Select(d => new MonthlyLoanCount(
                d.Year,
                d.Month,
                raw.FirstOrDefault(r => r.Year == d.Year && r.Month == d.Month)?.Count ?? 0))
            .ToList();

        var recentAccounts = (await _accounts.GetRecentAsync(3, ct))
            .Select(a => new AccountSummaryDto(a.Id, a.FullName, a.Role, a.Status))
            .ToList();

        var recentLogs = (await _logs.GetRecentAsync(4, ct))
            .Select(l => new SystemLogDto(l.Timestamp, l.Account.FullName, l.Account.Role, l.Action))
            .ToList();

        var policy = await _policies.GetCurrentAsync(ct) ?? PolicyConfiguration.Default;

        return new AdminDashboardDto(
            active, overdue, fines, accounts, monthly, recentAccounts, recentLogs,
            new PolicyDto(policy.MaxLoanDays, policy.MaxBooksPerReader, policy.MaxReservations, policy.FinePerDay));
    }
}
