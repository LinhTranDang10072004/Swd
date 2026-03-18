using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public int? UserId { get; set; }

    public int? InsuranceId { get; set; }

    public string FullName { get; set; } = null!;

    public string? NationalId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? BloodType { get; set; }

    public string? Allergies { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<HospitalAdmissionRequest> HospitalAdmissionRequests { get; set; } = new List<HospitalAdmissionRequest>();

    public virtual InsuranceInfo? Insurance { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();

    public virtual User? User { get; set; }
}
