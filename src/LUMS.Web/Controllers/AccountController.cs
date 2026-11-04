using System.Security.Claims;
using LUMS.Core.Services.Interfaces;
using LUMS.Web.Models;
using LUMS.Web.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _auth;
    private readonly IWebHostEnvironment _env;

    public AccountController(IAuthService auth, IWebHostEnvironment env)
    {
        _auth = auth;
        _env = env;
    }

    // GET /Account/Login?returnUrl=
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(RoleHome.UrlFor(User.GetRoleName()));
        }

        return LoginView(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return LoginView(model);
        }

        var result = await _auth.LoginAsync(model.Identifier, model.Password, ct);
        if (!result.Succeeded || result.User is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Đăng nhập không thành công.");
            return LoginView(model);
        }

        var user = result.User;
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(LumsClaimTypes.Code, user.Code)
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = model.RememberMe });

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return LocalRedirect(RoleHome.UrlFor(user.Role.ToString()));
    }

    // GET /Account/Logout — chỉ đăng xuất qua POST, GET quay về trang chủ
    [HttpGet]
    public IActionResult Logout() => RedirectToAction("Index", "Home");

    // POST /Account/Logout
    [HttpPost]
    [ActionName("Logout")]
    public async Task<IActionResult> LogoutPost()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // GET /Account/Forbidden — trang 403 (AccessDeniedPath của cookie auth)
    public IActionResult Forbidden()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    private IActionResult LoginView(LoginViewModel model)
    {
        ViewData["ShowDemoAccounts"] = _env.IsDevelopment();
        return View("Login", model);
    }
}
