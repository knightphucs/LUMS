using LUMS.Core.Enums;
using LUMS.Core.Exceptions;

namespace LUMS.Core.Entities;

public class Loan
{
    public int Id { get; set; }

    public string CopyId { get; set; } = string.Empty;
    public Copy Copy { get; set; } = null!;

    public int ReaderId { get; set; }
    public Account Reader { get; set; } = null!;

    public int? LibrarianId { get; set; }
    public Account? Librarian { get; set; }

    public DateTime BorrowedAt { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnedDate { get; set; }
    public int RenewalCount { get; set; }
    public LoanStatus Status { get; set; }

    public int GetOverdueDays(DateOnly today) =>
        Status == LoanStatus.DaTra ? 0 : Math.Max(0, today.DayNumber - DueDate.DayNumber);

    public string? GetRenewBlockedReason(DateOnly today, bool hasPendingReservationByOthers)
    {
        if (Status == LoanStatus.DaTra) return "Sách đã được trả.";
        if (Status == LoanStatus.QuaHan || today > DueDate) return "Sách đã quá hạn, không thể gia hạn.";
        if (hasPendingReservationByOthers) return "Sách đang có người đặt trước.";
        return null;
    }

    public void Renew(DateOnly today, int loanDays, bool hasPendingReservationByOthers)
    {
        var reason = GetRenewBlockedReason(today, hasPendingReservationByOthers);
        if (reason is not null)
        {
            throw new DomainException(reason);
        }

        DueDate = DueDate.AddDays(loanDays);
        RenewalCount++;
    }
}
