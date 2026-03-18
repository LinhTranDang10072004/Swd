using FinalProject.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Auth;

public static class RoleRedirect
{
    public static IActionResult ToDashboard(Controller controller, string? role)
    {
        return role switch
        {
            AppRoles.Receptionist => controller.RedirectToAction("Receptionist", "Dashboard"),
            AppRoles.Doctor => controller.RedirectToAction("Doctor", "Dashboard"),
            AppRoles.Pharmacist => controller.RedirectToAction("Pharmacist", "Dashboard"),
            AppRoles.Manager => controller.RedirectToAction("Manager", "Dashboard"),
            AppRoles.Patient => controller.RedirectToAction("Patient", "Dashboard"),
            _ => controller.RedirectToAction("Index", "Dashboard"),
        };
    }
}

