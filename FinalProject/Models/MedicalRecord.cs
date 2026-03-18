using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class MedicalRecord
{
    public int RecordId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime VisitDate { get; set; }

    public string? ChiefComplaint { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
