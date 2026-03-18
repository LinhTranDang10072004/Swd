using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class LabOrder
{
    public int OrderId { get; set; }

    public int RecordId { get; set; }

    public int DoctorId { get; set; }

    public DateTime OrderedAt { get; set; }

    public string Priority { get; set; } = null!;

    public string? ClinicalNotes { get; set; }

    public string Status { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<LabOrderDetail> LabOrderDetails { get; set; } = new List<LabOrderDetail>();

    public virtual MedicalRecord Record { get; set; } = null!;
}
