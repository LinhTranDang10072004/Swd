using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class HospitalAdmissionRequest
{
    public int RequestId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int? AppointmentId { get; set; }

    public int? RoomId { get; set; }

    public string AdmissionReason { get; set; } = null!;

    public DateOnly? RequestedDate { get; set; }

    public DateTime? AdmissionDate { get; set; }

    public DateTime? DischargeDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual Room? Room { get; set; }
}
