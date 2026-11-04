using LUMS.Core.Dtos;

namespace LUMS.Web.Models;

public class MyLoansViewModel
{
    public IReadOnlyList<LoanDto> Loans { get; init; } = Array.Empty<LoanDto>();

    public IReadOnlyList<ReservationDto> Reservations { get; init; } = Array.Empty<ReservationDto>();

    public decimal TotalFine => Loans.Sum(l => l.OverdueFine);
}
