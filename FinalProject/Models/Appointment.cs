using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int? ReceptionistId { get; set; }

    public int? MedicalRecordId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int? QueueNumber { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public string? CancelReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CancellationLog> CancellationLogs { get; set; } = new List<CancellationLog>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<HospitalAdmissionRequest> HospitalAdmissionRequests { get; set; } = new List<HospitalAdmissionRequest>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual MedicalRecord? MedicalRecord { get; set; }

    public virtual ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();

    public virtual Patient Patient { get; set; } = null!;

    public virtual Receptionist? Receptionist { get; set; }
}
