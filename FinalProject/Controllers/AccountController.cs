using System.Security.Claims;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

public class AccountController : Controller
{
    private readonly HospitalManagementContext _db;

    public AccountController(HospitalManagementContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var input = vm.UsernameOrEmail.Trim();
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                (u.Username == input || u.Email == input) &&
                u.Status == "ACTIVE");

        if (user is null || user.Password != vm.Password)
        {
            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, (user.Role ?? "").Trim().ToUpperInvariant()),
            new("full_name", user.FullName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = vm.ReturnUrl
            });

        if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
        {
            return Redirect(vm.ReturnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}

