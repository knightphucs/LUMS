using LUMS.Core.Services.Interfaces;
using LUMS.Web.Models;
using LUMS.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers;

[Authorize(Roles = AppRoles.Reader)]
public class ReaderController : Controller
{
    private readonly ILoanService _loans;
    private readonly IReservationService _reservations;

    public ReaderController(ILoanService loans, IReservationService reservations)
    {
        _loans = loans;
        _reservations = reservations;
    }

    // GET /Reader/MyLoans
    public async Task<IActionResult> MyLoans(CancellationToken ct)
    {
        var readerId = User.GetAccountId();
        var model = new MyLoansViewModel
        {
            Loans = await _loans.GetActiveLoansAsync(readerId, ct),
            Reservations = await _reservations.GetMyReservationsAsync(readerId, ct)
        };
        return View(model);
    }
}
