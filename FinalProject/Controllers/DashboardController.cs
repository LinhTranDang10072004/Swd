using FinalProject.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        var role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        return RoleRedirect.ToDashboard(this, role);
    }

    [Authorize(Roles = AppRoles.Receptionist)]
    public IActionResult Receptionist() => View();

    [Authorize(Roles = AppRoles.Doctor)]
    public IActionResult Doctor() => View();

    [Authorize(Roles = AppRoles.Pharmacist)]
    public IActionResult Pharmacist() => View();

    [Authorize(Roles = AppRoles.Manager)]
    public IActionResult Manager() => View();

    [Authorize(Roles = AppRoles.Patient)]
    public IActionResult Patient() => View();
}

