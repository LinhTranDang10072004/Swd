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
    private readonly IEmailSender _email;
    private const int SlotMinutes = 30;

    public PatientAppointmentsController(HospitalManagementContext db, IEmailSender email)
    {
        _db = db;
        _email = email;
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

        var slotOk = await IsSlotAvailable(vm.DoctorId, vm.AppointmentDate);
        if (!slotOk)
        {
            ModelState.AddModelError(nameof(vm.AppointmentDate), "Khung giờ đã được đặt hoặc bác sĩ không có lịch làm việc.");
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

        var appt = new Appointment
        {
            PatientId = patientId.Value,
            DoctorId = vm.DoctorId,
            AppointmentDate = NormalizeToSlot(vm.AppointmentDate),
            Reason = string.IsNullOrWhiteSpace(vm.Reason) ? null : vm.Reason.Trim(),
            Status = "PENDING",
            CreatedAt = DateTime.Now
        };

        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync();

        await TrySendConfirmationEmail(patientId.Value, appt);

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
        var slots = await GetAvailableSlotsForDay(doctorId, DateOnly.FromDateTime(date));
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

    private async Task TrySendConfirmationEmail(int patientId, Appointment appt)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            email = await _db.Patients.AsNoTracking()
                .Where(p => p.PatientId == patientId)
                .Select(p => p.Email)
                .FirstOrDefaultAsync();
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var doctorName = await _db.Doctors.AsNoTracking()
            .Where(d => d.DoctorId == appt.DoctorId)
            .Select(d => d.User.FullName)
            .FirstOrDefaultAsync();

        var subject = "Xác nhận đặt lịch khám";
        var body = $@"
            <div style=""font-family:Arial,sans-serif;font-size:14px"">
              <h3>Xác nhận đặt lịch khám</h3>
              <p>Bạn đã đặt lịch khám thành công.</p>
              <ul>
                <li><b>Bác sĩ</b>: {System.Net.WebUtility.HtmlEncode(doctorName ?? "")}</li>
                <li><b>Thời gian</b>: {appt.AppointmentDate:dd/MM/yyyy HH:mm}</li>
                <li><b>Trạng thái</b>: {System.Net.WebUtility.HtmlEncode(appt.Status)}</li>
              </ul>
              <p>Nếu thông tin không đúng, vui lòng liên hệ lễ tân để được hỗ trợ.</p>
            </div>";

        try
        {
            await _email.SendAsync(email, subject, body);
        }
        catch
        {
            // Ignore email errors (config may be missing)
        }
    }

    private async Task<bool> IsSlotAvailable(int doctorId, DateTime slotStart)
    {
        var day = DateOnly.FromDateTime(slotStart);
        var slots = await GetAvailableSlotsForDay(doctorId, day);
        var normalized = NormalizeToSlot(slotStart);
        return slots.Any(s => s == normalized);
    }

    private async Task<List<DateTime>> GetAvailableSlotsForDay(int doctorId, DateOnly day)
    {
        var doctor = await _db.Doctors.AsNoTracking()
            .Where(d => d.DoctorId == doctorId)
            .Select(d => new { d.DoctorId, d.UserId })
            .FirstOrDefaultAsync();
        if (doctor is null)
        {
            return new List<DateTime>();
        }

        var schedules = await _db.StaffSchedules.AsNoTracking()
            .Where(s => s.UserId == doctor.UserId && s.ScheduleDate == day && s.Status == "SCHEDULED")
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        if (schedules.Count == 0)
        {
            return new List<DateTime>();
        }

        var startOfDay = day.ToDateTime(TimeOnly.MinValue);
        var endOfDay = day.ToDateTime(TimeOnly.MaxValue);

        var booked = await _db.Appointments.AsNoTracking()
            .Where(a => a.DoctorId == doctorId
                        && a.AppointmentDate >= startOfDay
                        && a.AppointmentDate <= endOfDay
                        && a.Status != "CANCELLED")
            .Select(a => a.AppointmentDate)
            .ToListAsync();

        var bookedSlots = new HashSet<DateTime>(booked.Select(NormalizeToSlot));
        var available = new List<DateTime>();

        foreach (var sch in schedules)
        {
            var start = day.ToDateTime(sch.StartTime);
            var end = day.ToDateTime(sch.EndTime);
            start = NormalizeToSlot(start);

            for (var t = start; t.AddMinutes(SlotMinutes) <= end; t = t.AddMinutes(SlotMinutes))
            {
                if (!bookedSlots.Contains(t) && t >= DateTime.Now.AddMinutes(-SlotMinutes))
                {
                    available.Add(t);
                }
            }
        }

        return available.Distinct().OrderBy(x => x).ToList();
    }

    private static DateTime NormalizeToSlot(DateTime dt)
    {
        var truncated = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        var minute = truncated.Minute - (truncated.Minute % SlotMinutes);
        return new DateTime(truncated.Year, truncated.Month, truncated.Day, truncated.Hour, minute, 0);
    }
}

