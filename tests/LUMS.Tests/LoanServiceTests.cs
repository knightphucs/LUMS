using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Exceptions;
using LUMS.Core.Services;
using LUMS.Tests.Fakes;

namespace LUMS.Tests;

public class LoanServiceTests
{
    private const int ReaderId = 10;
    private const int BookId = 1;

    private readonly FakeLoanRepository _loans = new();
    private readonly FakeReservationRepository _reservations = new();
    private readonly FakePolicyRepository _policies = new();
    private readonly FakeSystemLogRepository _logs = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeClock _clock = new(new DateTime(2026, 10, 12, 9, 0, 0));

    private LoanService CreateSut() => new(_loans, _reservations, _policies, _logs, _uow, _clock);

    private Loan AddLoan(int id, int dueInDays, int readerId = ReaderId)
    {
        var loan = new Loan
        {
            Id = id,
            CopyId = $"LUMS-CS101-00{id}",
            ReaderId = readerId,
            BorrowedAt = _clock.Now.AddDays(dueInDays - 14),
            DueDate = _clock.Today.AddDays(dueInDays),
            Status = dueInDays < 0 ? LoanStatus.QuaHan : LoanStatus.DangMuon,
            Copy = new Copy
            {
                Id = $"LUMS-CS101-00{id}",
                BookTitleId = BookId,
                BookTitle = new BookTitle { Id = BookId, Title = "Lập trình C# Cơ bản" }
            }
        };
        _loans.Items.Add(loan);
        return loan;
    }

    [Fact]
    public async Task RenewAsync_Success_UsesPolicyDays_WritesLogAndSavesOnce()
    {
        _policies.Items.Add(new PolicyConfiguration { MaxLoanDays = 7, MaxBooksPerReader = 5, FinePerDay = 5000 });
        var loan = AddLoan(id: 1, dueInDays: 3);

        var result = await CreateSut().RenewAsync(1, ReaderId);

        Assert.Equal(_clock.Today.AddDays(3 + 7), result.NewDueDate);
        Assert.Equal(1, result.RenewalCount);
        Assert.Equal(result.NewDueDate, loan.DueDate);
        Assert.Single(_logs.Items);
        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task RenewAsync_WhenNoPolicyConfigured_FallsBackToDefault14Days()
    {
        AddLoan(id: 1, dueInDays: 0);

        var result = await CreateSut().RenewAsync(1, ReaderId);

        Assert.Equal(_clock.Today.AddDays(14), result.NewDueDate);
    }

    [Fact]
    public async Task RenewAsync_WhenAnotherReaderReserved_ThrowsAndDoesNotSave()
    {
        AddLoan(id: 1, dueInDays: 3);
        _reservations.Pending.Add((BookId, ReaderId: 99));

        await Assert.ThrowsAsync<DomainException>(() => CreateSut().RenewAsync(1, ReaderId));

        Assert.Equal(0, _uow.SaveCount);
        Assert.Empty(_logs.Items);
    }

    [Fact]
    public async Task RenewAsync_WhenOwnReservation_DoesNotBlock()
    {
        AddLoan(id: 1, dueInDays: 3);
        _reservations.Pending.Add((BookId, ReaderId: ReaderId));

        var result = await CreateSut().RenewAsync(1, ReaderId);

        Assert.Equal(1, result.RenewalCount);
    }

    [Fact]
    public async Task RenewAsync_WhenOverdue_Throws()
    {
        AddLoan(id: 1, dueInDays: -2);

        await Assert.ThrowsAsync<DomainException>(() => CreateSut().RenewAsync(1, ReaderId));
    }

    [Fact]
    public async Task RenewAsync_WhenLoanBelongsToSomeoneElse_ThrowsForbidden()
    {
        AddLoan(id: 1, dueInDays: 3, readerId: 77);

        await Assert.ThrowsAsync<ForbiddenException>(() => CreateSut().RenewAsync(1, ReaderId));
    }

    [Fact]
    public async Task RenewAsync_WhenLoanMissing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => CreateSut().RenewAsync(404, ReaderId));
    }

    [Fact]
    public async Task GetActiveLoansAsync_MarksRenewableBlockedAndOverdueRows()
    {
        _policies.Items.Add(new PolicyConfiguration { MaxLoanDays = 14, MaxBooksPerReader = 5, FinePerDay = 5000 });
        AddLoan(id: 1, dueInDays: 3);
        AddLoan(id: 2, dueInDays: -3);

        var rows = await CreateSut().GetActiveLoansAsync(ReaderId);

        var ok = rows.Single(r => r.Id == 1);
        var overdue = rows.Single(r => r.Id == 2);
        Assert.True(ok.CanRenew);
        Assert.False(overdue.CanRenew);
        Assert.True(overdue.IsOverdue);
        Assert.Equal(15000m, overdue.OverdueFine);
    }
}
