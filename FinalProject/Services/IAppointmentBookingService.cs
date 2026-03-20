using FinalProject.Models;

namespace FinalProject.Services;

public interface IAppointmentBookingService
{
    Task<ConfirmBookingResult> ConfirmBookingAsync(
        int patientId,
        int doctorId,
        DateTime slotTime,
        string? reason,
        CancellationToken cancellationToken = default);

    Task<bool> CheckRealTimeAvailabilityAsync(
        int doctorId,
        DateTime slotTime,
        CancellationToken cancellationToken = default);

    Task<List<DateTime>> GetAvailableSlotsForDayAsync(
        int doctorId,
        DateOnly day,
        CancellationToken cancellationToken = default);
}

public sealed record ConfirmBookingResult(bool Success, string? ErrorMessage, Appointment? Appointment)
{
    public static ConfirmBookingResult SlotTaken() => new(false, "Khung giờ này không còn khả dụng (vừa có người đặt).", null);
    public static ConfirmBookingResult InvalidSlot(string message) => new(false, message, null);
    public static ConfirmBookingResult Ok(Appointment appointment) => new(true, null, appointment);
}

