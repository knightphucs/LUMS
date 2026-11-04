using LUMS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers;

public class HomeController : Controller
{
    // GET / — lưới "Sách mới cập nhật" do View Component LatestBooks tự nạp
    public IActionResult Index() => View();

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
