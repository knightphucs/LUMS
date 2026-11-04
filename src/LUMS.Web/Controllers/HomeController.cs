using LUMS.Core.Services.Interfaces;
using LUMS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers;

public class HomeController : Controller
{
    private readonly IBookService _books;

    public HomeController(IBookService books)
    {
        _books = books;
    }

    // GET /
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var model = new HomeIndexViewModel
        {
            LatestBooks = await _books.GetLatestAsync(4, ct)
        };
        return View(model);
    }

    // GET /Error?code=404 — được gọi lại bởi UseExceptionHandler / UseStatusCodePagesWithReExecute
    [Route("/Error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public IActionResult Error(int? code)
    {
        var model = new ErrorViewModel
        {
            Code = code ?? 500,
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        Response.StatusCode = model.Code;
        return View(model);
    }
}
