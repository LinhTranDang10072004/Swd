using FinalProject.Auth;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Receptionist)]
public class PatientsController : Controller
{
    private readonly HospitalManagementContext _db;

    public PatientsController(HospitalManagementContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Create(string? nationalId = null, string? next = null, string? returnUrl = null)
    {
        ViewData["Next"] = next;
        return View(new PatientRegistrationViewModel
        {
            NationalId = nationalId,
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PatientRegistrationViewModel vm, string? next = null)
    {
        ViewData["Next"] = next;

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var normalizedNationalId = vm.NationalId?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedNationalId))
        {
            var exists = await _db.Patients.AsNoTracking()
                .AnyAsync(p => p.NationalId != null && p.NationalId == normalizedNationalId);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.NationalId), "CMND/CCCD đã tồn tại trong hệ thống.");
                return View(vm);
            }
        }

        var patient = new Patient
        {
            FullName = vm.FullName.Trim(),
            NationalId = normalizedNationalId,
            DateOfBirth = vm.DateOfBirth.HasValue ? DateOnly.FromDateTime(vm.DateOfBirth.Value) : null,
            Gender = string.IsNullOrWhiteSpace(vm.Gender) ? null : vm.Gender.Trim(),
            Address = string.IsNullOrWhiteSpace(vm.Address) ? null : vm.Address.Trim(),
            Phone = string.IsNullOrWhiteSpace(vm.Phone) ? null : vm.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(vm.Email) ? null : vm.Email.Trim(),
            CreatedAt = DateTime.Now
        };

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        if (string.Equals(next, "appointment", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Create", "Appointments", new { patientId = patient.PatientId });
        }

        if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
        {
            return Redirect(vm.ReturnUrl);
        }

        return RedirectToAction("Receptionist", "Dashboard");
    }
}

