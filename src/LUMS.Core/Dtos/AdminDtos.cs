using LUMS.Core.Enums;

namespace LUMS.Core.Dtos;

public record MonthlyLoanCount(int Year, int Month, int Count);

public record AccountSummaryDto(int Id, string FullName, Role Role, AccountStatus Status);

public record SystemLogDto(DateTime Timestamp, string AccountName, Role AccountRole, string Action);

public record PolicyDto(int MaxLoanDays, int MaxBooksPerReader, int MaxReservations, decimal FinePerDay);

public record UpdatePolicyRequest(int MaxLoanDays, int MaxBooksPerReader, int MaxReservations, decimal FinePerDay);

public record AdminDashboardDto(
    int ActiveLoans,
    int OverdueLoans,
    decimal FinesCollectedThisMonth,
    int TotalAccounts,
    IReadOnlyList<MonthlyLoanCount> MonthlyLoans,
    IReadOnlyList<AccountSummaryDto> RecentAccounts,
    IReadOnlyList<SystemLogDto> RecentLogs,
    PolicyDto Policy);
