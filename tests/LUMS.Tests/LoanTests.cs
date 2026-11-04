using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Exceptions;

namespace LUMS.Tests;

public class LoanTests
{
    private static readonly DateOnly Today = new(2026, 10, 12);

    private static Loan NewLoan(DateOnly dueDate, LoanStatus status = LoanStatus.DangMuon) => new()
    {
        Id = 1,
        CopyId = "LUMS-CS101-001",
        ReaderId = 10,
        DueDate = dueDate,
        Status = status
    };

    [Fact]
    public void Renew_WhenAllowed_ExtendsDueDateAndCountsRenewal()
    {
        var loan = NewLoan(Today.AddDays(2));

        loan.Renew(Today, loanDays: 14, hasPendingReservationByOthers: false);

        Assert.Equal(Today.AddDays(16), loan.DueDate);
        Assert.Equal(1, loan.RenewalCount);
    }

    [Fact]
    public void Renew_WhenOverdue_ThrowsDomainException()
    {
        var loan = NewLoan(Today.AddDays(-1));

        var ex = Assert.Throws<DomainException>(() => loan.Renew(Today, 14, false));

        Assert.Contains("quá hạn", ex.Message);
        Assert.Equal(0, loan.RenewalCount);
    }

    [Fact]
    public void Renew_WhenReservedByAnotherReader_ThrowsDomainException()
    {
        var loan = NewLoan(Today.AddDays(5));

        var ex = Assert.Throws<DomainException>(() => loan.Renew(Today, 14, hasPendingReservationByOthers: true));

        Assert.Contains("đặt trước", ex.Message);
        Assert.Equal(Today.AddDays(5), loan.DueDate);
    }

    [Fact]
    public void Renew_OnDueDateItself_IsStillAllowed()
    {
        var loan = NewLoan(Today);

        Assert.Null(loan.GetRenewBlockedReason(Today, false));
    }

    [Fact]
    public void GetOverdueDays_CountsDaysAfterDueDate()
    {
        Assert.Equal(3, NewLoan(Today.AddDays(-3)).GetOverdueDays(Today));
        Assert.Equal(0, NewLoan(Today.AddDays(3)).GetOverdueDays(Today));
        Assert.Equal(0, NewLoan(Today.AddDays(-3), LoanStatus.DaTra).GetOverdueDays(Today));
    }
}
