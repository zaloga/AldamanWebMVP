using Aldaman.Services.Interfaces;
using Aldaman.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public class AccountController : Controller
{
    private IAccountService AccountService { get; }

    public AccountController(IAccountService accountService)
    {
        AccountService = accountService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        ViewData["ReturnUrl"] = request.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await AccountService.LoginAsync(request.Email, request.Password, request.RememberMe);

        if (result.Succeeded)
        {
            return RedirectToLocal(request.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Účet je zablokován. Zkuste to prosím později.");
            return View(request);
        }

        ModelState.AddModelError(string.Empty, "Neplatné přihlašovací údaje.");
        return View(request);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await AccountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
