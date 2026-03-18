using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class LabOrderDetail
{
    public int DetailId { get; set; }

    public int OrderId { get; set; }

    public int TestId { get; set; }

    public string? Result { get; set; }

    public string? ResultUnit { get; set; }

    public string? ReferenceRange { get; set; }

    public bool? IsAbnormal { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? ResultedAt { get; set; }

    public int? VerifiedBy { get; set; }

    public virtual LabOrder Order { get; set; } = null!;

    public virtual LabTest Test { get; set; } = null!;

    public virtual Doctor? VerifiedByNavigation { get; set; }
}
