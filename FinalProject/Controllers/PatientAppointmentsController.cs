using System.Security.Claims;
using FinalProject.Auth;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Patient)]
public class PatientAppointmentsController : Controller
{
    private readonly HospitalManagementContext _db;
    private readonly IAppointmentBookingService _booking;

    public PatientAppointmentsController(HospitalManagementContext db, IAppointmentBookingService booking)
    {
        _db = db;
        _booking = booking;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        var items = await _db.Appointments.AsNoTracking()
            .Where(a => a.PatientId == patientId.Value)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .OrderByDescending(a => a.AppointmentDate)
            .Take(100)
            .ToListAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Book(int? doctorId = null)
    {
        var specialties = await _db.Specialties.AsNoTracking()
            .OrderBy(s => s.SpecialtyName)
            .Select(s => new SelectListItem { Value = s.SpecialtyId.ToString(), Text = s.SpecialtyName })
            .ToListAsync();

        var vm = new PatientAppointmentBookViewModel
        {
            Specialties = specialties,
            Doctors = new List<SelectListItem>(),
            PreselectedDoctorId = doctorId
        };

        if (doctorId.HasValue)
        {
            var doc = await _db.Doctors.AsNoTracking()
                .Where(d => d.DoctorId == doctorId.Value)
                .Select(d => new { d.DoctorId, d.SpecialtyId })
                .FirstOrDefaultAsync();
            if (doc is not null)
            {
                vm.SpecialtyId = doc.SpecialtyId;
                vm.DoctorId = doc.DoctorId;
                vm.Doctors = await _db.Doctors.AsNoTracking()
                    .Include(d => d.User)
                    .Where(d => d.SpecialtyId == doc.SpecialtyId)
                    .OrderBy(d => d.User.FullName)
                    .Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = d.User.FullName })
                    .ToListAsync();
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(PatientAppointmentBookViewModel vm)
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            vm.Specialties = await _db.Specialties.AsNoTracking()
                .OrderBy(s => s.SpecialtyName)
                .Select(s => new SelectListItem { Value = s.SpecialtyId.ToString(), Text = s.SpecialtyName })
                .ToListAsync();
            vm.Doctors = await _db.Doctors.AsNoTracking()
                .Include(d => d.User)
                .Where(d => d.SpecialtyId == vm.SpecialtyId)
                .OrderBy(d => d.User.FullName)
                .Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = d.User.FullName })
                .ToListAsync();
            return View(vm);
        }

        var doctorOk = await _db.Doctors.AsNoTracking()
            .AnyAsync(d => d.DoctorId == vm.DoctorId && d.SpecialtyId == vm.SpecialtyId);
        if (!doctorOk)
        {
            ModelState.AddModelError(nameof(vm.DoctorId), "Bác sĩ không thuộc khoa/chuyên khoa đã chọn.");
            vm.Specialties = await _db.Specialties.AsNoTracking()
                .OrderBy(s => s.SpecialtyName)
                .Select(s => new SelectListItem { Value = s.SpecialtyId.ToString(), Text = s.SpecialtyName })
                .ToListAsync();
            vm.Doctors = await _db.Doctors.AsNoTracking()
                .Include(d => d.User)
                .Where(d => d.SpecialtyId == vm.SpecialtyId)
                .OrderBy(d => d.User.FullName)
                .Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = d.User.FullName })
                .ToListAsync();
            return View(vm);
        }

        var bookingResult = await _booking.ConfirmBookingAsync(
            patientId.Value,
            vm.DoctorId,
            vm.AppointmentDate,
            vm.Reason);
        if (!bookingResult.Success)
        {
            ModelState.AddModelError(nameof(vm.AppointmentDate), bookingResult.ErrorMessage ?? "Không thể đặt lịch cho khung giờ này.");
            vm.Specialties = await _db.Specialties.AsNoTracking()
                .OrderBy(s => s.SpecialtyName)
                .Select(s => new SelectListItem { Value = s.SpecialtyId.ToString(), Text = s.SpecialtyName })
                .ToListAsync();
            vm.Doctors = await _db.Doctors.AsNoTracking()
                .Include(d => d.User)
                .Where(d => d.SpecialtyId == vm.SpecialtyId)
                .OrderBy(d => d.User.FullName)
                .Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = d.User.FullName })
                .ToListAsync();
            return View(vm);
        }

        TempData["Success"] = "Đặt lịch khám thành công. Vui lòng kiểm tra email để xác nhận.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DoctorsBySpecialty(int specialtyId)
    {
        var doctors = await _db.Doctors.AsNoTracking()
            .Include(d => d.User)
            .Where(d => d.SpecialtyId == specialtyId)
            .OrderBy(d => d.User.FullName)
            .Select(d => new { id = d.DoctorId, name = d.User.FullName })
            .ToListAsync();

        return Json(doctors);
    }

    [HttpGet]
    public async Task<IActionResult> AvailableSlots(int doctorId, DateTime date)
    {
        var slots = await _booking.GetAvailableSlotsForDayAsync(doctorId, DateOnly.FromDateTime(date));
        var result = slots.Select(dt => new
        {
            value = dt.ToString("yyyy-MM-ddTHH:mm"),
            label = dt.ToString("dd/MM/yyyy HH:mm")
        });
        return Json(result);
    }

    private async Task<int?> GetCurrentPatientId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return null;
        }

        return await _db.Patients.AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => (int?)p.PatientId)
            .FirstOrDefaultAsync();
    }
}

