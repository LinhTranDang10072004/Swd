using System.Security.Claims;
using FinalProject.Auth;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Receptionist)]
public class ReceptionistController : Controller
{
    private readonly HospitalManagementContext _db;

    public ReceptionistController(HospitalManagementContext db)
    {
        _db = db;
    }

    public IActionResult Index() => RedirectToAction("Index", "Appointments");

    [HttpGet]
    public IActionResult Appointments()
    {
        return RedirectToAction("Index", "Appointments");
    }

    [HttpGet]
    public IActionResult NewAppointment() => View(new AppointmentLookupViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewAppointment(AppointmentLookupViewModel vm)
    {
        // Backward compatible: forward to the new flow
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var nationalId = vm.NationalId.Trim();
        var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.NationalId == nationalId);

        if (patient is null)
        {
            TempData["Info"] = "Không tìm thấy bệnh nhân theo CMND/CCCD. Vui lòng đăng ký bệnh nhân mới.";
            return RedirectToAction("Create", "Patients", new { nationalId, next = "appointment" });
        }

        return RedirectToAction("Create", "Appointments", new { patientId = patient.PatientId });
    }

    [HttpGet]
    public async Task<IActionResult> CreateAppointmentAfterRegister(string nationalId)
    {
        var nid = nationalId.Trim();
        var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.NationalId == nid);
        if (patient is null)
        {
            TempData["Info"] = "Không tìm thấy bệnh nhân theo CMND/CCCD. Vui lòng đăng ký bệnh nhân mới.";
            return RedirectToAction("Create", "Patients", new { nationalId = nid, next = "appointment" });
        }

        return RedirectToAction("Create", "Appointments", new { patientId = patient.PatientId });
    }

    [HttpGet]
    public IActionResult CreateAppointment(int patientId)
    {
        // Backward compatible: old URL -> new UI with Specialty -> Doctor -> Slots
        return RedirectToAction("Create", "Appointments", new { patientId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAppointment(AppointmentCreateViewModel vm)
    {
        // Prefer new flow; keep POST endpoint for compatibility by forwarding
        return RedirectToAction("Create", "Appointments", new { patientId = vm.PatientId });
    }

    [HttpGet]
    public IActionResult RegisterPatient(string? nationalId = null, string? returnUrl = null)
    {
        return RedirectToAction("Create", "Patients", new { nationalId, returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RegisterPatient(PatientRegistrationViewModel vm)
    {
        // This action is no longer used; keep for routing compatibility.
        return RedirectToAction("Create", "Patients", new { nationalId = vm.NationalId, returnUrl = vm.ReturnUrl });
    }

    private async Task<int?> GetReceptionistIdForCurrentUser()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return null;
        }

        return await _db.Receptionists
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Select(r => (int?)r.ReceptionistId)
            .FirstOrDefaultAsync();
    }
}

