using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int RecordId { get; set; }

    public int DoctorId { get; set; }

    public DateTime IssuedAt { get; set; }

    public string? DiagnosisNote { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();

    public virtual MedicalRecord Record { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }
}
