using LUMS.Core.Dtos;
using LUMS.Core.Interfaces.Services;
using LUMS.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers.Api;

[ApiController]
[ApiExceptionFilter]
[Route("api/loans")]
[Tags("Loans")]
[Authorize(Roles = AppRoles.Reader)]
public class LoansApiController : ControllerBase
{
    private readonly ILoanService _loans;

    public LoansApiController(ILoanService loans)
    {
        _loans = loans;
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<LoanDto>>> GetMine(CancellationToken ct) =>
        Ok(await _loans.GetActiveLoansAsync(User.GetAccountId(), ct));

    [HttpPost("{id:int}/renew")]
    public async Task<ActionResult<RenewLoanResult>> Renew(int id, CancellationToken ct) =>
        Ok(await _loans.RenewAsync(id, User.GetAccountId(), ct));
}
