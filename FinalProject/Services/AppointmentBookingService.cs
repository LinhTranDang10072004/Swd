using System.Data;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services;

public class AppointmentBookingService : IAppointmentBookingService
{
    private readonly HospitalManagementContext _db;
    private readonly IEmailSender _email;

    private const int SlotMinutes = 30;
    private const string PendingStatus = "PENDING";
    private const string CancelledStatus = "CANCELLED";
    private const string ScheduledShiftStatus = "SCHEDULED";

    public AppointmentBookingService(HospitalManagementContext db, IEmailSender email)
    {
        _db = db;
        _email = email;
    }

    public async Task<ConfirmBookingResult> ConfirmBookingAsync(
        int patientId,
        int doctorId,
        DateTime slotTime,
        string? reason,
        CancellationToken cancellationToken = default)
    {
        var normalizedSlot = NormalizeToSlot(slotTime);

        // Step 1 (P15/P16): validate that the slot is within the doctor's schedule.
        var slotWithinSchedule = await IsSlotWithinDoctorScheduleAsync(doctorId, normalizedSlot, cancellationToken);
        if (!slotWithinSchedule)
        {
            return ConfirmBookingResult.InvalidSlot("Khung giờ không còn hợp lệ hoặc bác sĩ không có lịch làm việc.");
        }

        // Step 2 (Row-locking / double-booking prevention):
        // Use SERIALIZABLE to prevent concurrent transactions from booking the same (doctorId, slotTime) range.
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var alreadyBooked = await _db.Appointments
            .Where(a =>
                a.DoctorId == doctorId
                && a.AppointmentDate == normalizedSlot
                && a.Status != CancelledStatus)
            .CountAsync(cancellationToken) > 0;

        if (alreadyBooked)
        {
            await tx.RollbackAsync(cancellationToken);
            return ConfirmBookingResult.SlotTaken();
        }

        // Step 3 (P17/P18): store appointment as Pending
        var appt = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDate = normalizedSlot,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            Status = PendingStatus,
            CreatedAt = DateTime.Now
        };

        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);

        // Step 4 (P19/P20): trigger notification (email) after successful persistence.
        await TrySendConfirmationEmailAsync(patientId, appt, cancellationToken);

        return ConfirmBookingResult.Ok(appt);
    }

    public async Task<bool> CheckRealTimeAvailabilityAsync(
        int doctorId,
        DateTime slotTime,
        CancellationToken cancellationToken = default)
    {
        var normalizedSlot = NormalizeToSlot(slotTime);

        var slotWithinSchedule = await IsSlotWithinDoctorScheduleAsync(doctorId, normalizedSlot, cancellationToken);
        if (!slotWithinSchedule)
        {
            return false;
        }

        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var alreadyBooked = await _db.Appointments
            .Where(a =>
                a.DoctorId == doctorId
                && a.AppointmentDate == normalizedSlot
                && a.Status != CancelledStatus)
            .CountAsync(cancellationToken) > 0;

        await tx.CommitAsync(cancellationToken);
        return !alreadyBooked;
    }

    public async Task<List<DateTime>> GetAvailableSlotsForDayAsync(
        int doctorId,
        DateOnly day,
        CancellationToken cancellationToken = default)
    {
        var doctor = await _db.Doctors.AsNoTracking()
            .Where(d => d.DoctorId == doctorId)
            .Select(d => new { d.DoctorId, d.UserId })
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
        {
            return new List<DateTime>();
        }

        var schedules = await _db.StaffSchedules.AsNoTracking()
            .Where(s =>
                s.UserId == doctor.UserId
                && s.ScheduleDate == day
                && s.Status == ScheduledShiftStatus)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        if (schedules.Count == 0)
        {
            return new List<DateTime>();
        }

        var startOfDay = day.ToDateTime(TimeOnly.MinValue);
        var endOfDay = day.ToDateTime(TimeOnly.MaxValue);

        var booked = await _db.Appointments.AsNoTracking()
            .Where(a =>
                a.DoctorId == doctorId
                && a.AppointmentDate >= startOfDay
                && a.AppointmentDate <= endOfDay
                && a.Status != CancelledStatus)
            .Select(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);

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

    private async Task<bool> IsSlotWithinDoctorScheduleAsync(
        int doctorId,
        DateTime normalizedSlot,
        CancellationToken cancellationToken)
    {
        var doctor = await _db.Doctors.AsNoTracking()
            .Where(d => d.DoctorId == doctorId)
            .Select(d => new { d.DoctorId, d.UserId })
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
        {
            return false;
        }

        var day = DateOnly.FromDateTime(normalizedSlot);
        var schedules = await _db.StaffSchedules.AsNoTracking()
            .Where(s =>
                s.UserId == doctor.UserId
                && s.ScheduleDate == day
                && s.Status == ScheduledShiftStatus)
            .ToListAsync(cancellationToken);

        if (schedules.Count == 0)
        {
            return false;
        }

        foreach (var sch in schedules)
        {
            var start = day.ToDateTime(sch.StartTime);
            var end = day.ToDateTime(sch.EndTime);
            start = NormalizeToSlot(start);

            for (var t = start; t.AddMinutes(SlotMinutes) <= end; t = t.AddMinutes(SlotMinutes))
            {
                if (t == normalizedSlot && t >= DateTime.Now.AddMinutes(-SlotMinutes))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private async Task TrySendConfirmationEmailAsync(
        int patientId,
        Appointment appt,
        CancellationToken cancellationToken)
    {
        var email = await _db.Patients.AsNoTracking()
            .Where(p => p.PatientId == patientId)
            .Select(p => p.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var doctorName = await _db.Doctors.AsNoTracking()
            .Where(d => d.DoctorId == appt.DoctorId)
            .Select(d => d.User.FullName)
            .FirstOrDefaultAsync(cancellationToken);

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
            // Ignore email errors (config may be missing).
        }
    }

    private static DateTime NormalizeToSlot(DateTime dt)
    {
        var truncated = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        var minute = truncated.Minute - (truncated.Minute % SlotMinutes);
        return new DateTime(truncated.Year, truncated.Month, truncated.Day, truncated.Hour, minute, 0);
    }
}

