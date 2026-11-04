using LUMS.Core.Dtos;
using LUMS.Core.Exceptions;
using LUMS.Core.Services.Interfaces;
using LUMS.Web.Models;
using LUMS.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly IDashboardService _dashboard;
    private readonly IPolicyService _policies;

    public AdminController(IDashboardService dashboard, IPolicyService policies)
    {
        _dashboard = dashboard;
        _policies = policies;
    }

    // GET /Admin/Dashboard
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var data = await _dashboard.GetAsync(ct);
        var model = new DashboardViewModel
        {
            Data = data,
            Policy = new PolicyForm
            {
                MaxLoanDays = data.Policy.MaxLoanDays,
                MaxBooksPerReader = data.Policy.MaxBooksPerReader,
                FinePerDay = data.Policy.FinePerDay
            }
        };
        return View(model);
    }

    // POST /Admin/SavePolicy
    [HttpPost]
    public async Task<IActionResult> SavePolicy([Bind(Prefix = "Policy")] PolicyForm policy, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var current = await _policies.GetCurrentAsync(ct);
                await _policies.UpdateAsync(
                    new UpdatePolicyRequest(policy.MaxLoanDays, policy.MaxBooksPerReader, current.MaxReservations, policy.FinePerDay),
                    User.GetAccountId(),
                    ct);

                TempData["Flash"] = "Đã lưu cấu hình quy định.";
                return RedirectToAction(nameof(Dashboard));
            }
            catch (DomainException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        var model = new DashboardViewModel
        {
            Data = await _dashboard.GetAsync(ct),
            Policy = policy
        };
        return View(nameof(Dashboard), model);
    }
}
