using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;
using FinalProject.ExternalSystem;


namespace SWD392.Controllers
{
    public class DoctorScheduleController : Controller
    {
        private readonly HospitalManagementContext _context;
        private readonly EmailService _emailService;

        public DoctorScheduleController(HospitalManagementContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Dummy data cho Bác sĩ đang đăng nhập
        private int GetCurrentDoctorId() => 1;

        // Operation 1: getAppointmentSchedule (Main Sequence Bước 1, 2, 3)
        public async Task<IActionResult> Index(int? doctorId, DateTime? selectedDate)
        {
            int currentDoctorId = doctorId ?? GetCurrentDoctorId();

            DateTime today = DateTime.Today;

            var query = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == currentDoctorId
                         );

            if (selectedDate.HasValue)
            {
                // 👉 DAY MODE
                DateTime date = selectedDate.Value.Date;

                query = query.Where(a => a.AppointmentDate.Date == date);

                ViewBag.ViewMode = "Day";
                ViewBag.SelectedDate = date.ToString("yyyy-MM-dd");
            }
            else
            {
                // 👉 WEEK MODE (FIX: start from Monday)
                var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                var startOfWeek = today.AddDays(-diff).Date;
                var endOfWeek = startOfWeek.AddDays(7);

                query = query.Where(a => a.AppointmentDate >= startOfWeek
                                      && a.AppointmentDate < endOfWeek);

                ViewBag.ViewMode = "Week";
                ViewBag.SelectedDate = "";

                // 👉 Hiển thị range tuần
                ViewBag.WeekRange = $"{startOfWeek:dd/MM/yyyy} - {endOfWeek.AddDays(-1):dd/MM/yyyy}";
            }

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            if (!appointments.Any())
            {
                ViewBag.Message = "No appointments available for the selected time range.";
            }

            return View(appointments);
        }

        // GET: Hiển thị form dời lịch
        public async Task<IActionResult> Reschedule(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            // A3 – Không cho sửa nếu đã xảy ra / completed
            if (appointment.Status == "Completed" || appointment.AppointmentDate < DateTime.Now)
            {
                TempData["Error"] = "The appointment has already occurred and cannot be changed.";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        // POST: Xử lý dời lịch (Operation 2, 3, 4)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule(int id, DateTime newTime)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            // ❌ A3 – Không cho sửa nếu đã xảy ra / completed
            if (appointment.Status == "Completed" || appointment.AppointmentDate < DateTime.Now)
            {
                TempData["Error"] = "The appointment has already occurred and cannot be changed.";
                return RedirectToAction(nameof(Index));
            }

            // ❌ A4 – Thời gian không hợp lệ
            if (newTime <= DateTime.Now)
            {
                TempData["Error"] = "Invalid time. Please select a future time.";
                return RedirectToAction(nameof(Reschedule), new { id });
            }

            // ❌ Check conflict (cùng doctor, trùng giờ)
            var isConflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appointment.DoctorId &&
                a.AppointmentId != id &&
                a.AppointmentDate == newTime &&
                a.Status.ToLower() != "cancelled"
            );

            if (isConflict)
            {
                TempData["Error"] = "The selected time is already booked. Please choose another time.";
                return RedirectToAction(nameof(Reschedule), new { id});
            }

            // ✅ UPDATE
            appointment.AppointmentDate = newTime;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // ✅ Gửi email (mock)
            try
            {
                await _emailService.SendEmailAsync(
    appointment.Patient.Email,
    "Appointment Rescheduled",
    $@"
    <h3>Appointment Rescheduled</h3>
    <p>Dear {appointment.Patient.FullName},</p>
    <p>Your appointment has been <b>rescheduled</b>.</p>
    <p><b>New Time:</b> {newTime:dd/MM/yyyy HH:mm}</p>
    <br/>
    <p>Thank you.</p>
    "
);
            }
            catch
            {
                // Exception: Email service lỗi
                // (log lại nếu cần)
            }

            TempData["Success"] = "Appointment rescheduled successfully.";

            return RedirectToAction(nameof(Index));
        }

       

        // GET: Hiển thị form xác nhận hủy lịch và nhập lý do
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();
            if (appointment.Status == "Cancelled")
            {
                TempData["Error"] = "This appointment is already cancelled.";
                return RedirectToAction(nameof(Index));
            }

            // Alternative Sequence A3: Completed or Already Occurred
            if (appointment.Status == "Completed" || appointment.AppointmentDate < DateTime.Now)
            {
                TempData["Error"] = "The appointment has already occurred and cannot be canceled.";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        // POST: Xử lý hủy lịch (Operation 2 & 4)
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id, string cancelReason)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient) // ✅ phải include để lấy email
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            // ❌ A3 – Không cho hủy nếu đã xảy ra
            if (appointment.Status == "Completed" || appointment.AppointmentDate < DateTime.Now)
            {
                TempData["Error"] = "The appointment has already occurred and cannot be canceled.";
                return RedirectToAction(nameof(Index));
            }

            // ❌ Validate reason (optional nhưng nên có)
            if (string.IsNullOrWhiteSpace(cancelReason))
            {
                TempData["Error"] = "Cancel reason is required.";
                return RedirectToAction(nameof(Index));
            }

            // ✅ UPDATE
            appointment.Status = "Cancelled";
            appointment.CancelReason = cancelReason;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // ✅ GỬI EMAIL
            try
            {
                await _emailService.SendEmailAsync(
                    appointment.Patient.Email,
                    "Appointment Cancelled",
                    $@"
            <h3>Appointment Cancelled</h3>
            <p>Dear {appointment.Patient.FullName},</p>

            <p>Your appointment has been <b>cancelled</b>.</p>

            <p><b>Original Time:</b> {appointment.AppointmentDate:dd/MM/yyyy HH:mm}</p>
            <p><b>Reason:</b> {cancelReason}</p>

            <br/>
            <p>Please contact us if you want to reschedule.</p>

            <p>Thank you.</p>
            "
                );
            }
            catch (Exception ex)
            {
                // ⚠️ Email lỗi → không fail hệ thống
                Console.WriteLine(ex.Message);
            }

            TempData["Success"] = "Appointment cancelled successfully. Patient has been notified via email.";

            return RedirectToAction(nameof(Index));
        }
    }
}
