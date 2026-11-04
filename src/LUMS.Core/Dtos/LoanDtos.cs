using LUMS.Core.Enums;

namespace LUMS.Core.Dtos;

public record LoanDto(
    int Id,
    string CopyId,
    string BookTitle,
    DateOnly BorrowedDate,
    DateOnly DueDate,
    int RenewalCount,
    bool IsOverdue,
    decimal OverdueFine,
    string? RenewBlockedReason)
{
    public bool CanRenew => RenewBlockedReason is null;
}

public record RenewLoanResult(int LoanId, DateOnly NewDueDate, int RenewalCount);

public record ReservationDto(int Id, string BookTitle, ReservationStatus Status, int QueuePosition, DateOnly? HoldUntil);
