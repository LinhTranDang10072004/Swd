using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class LabTest
{
    public int TestId { get; set; }

    public string TestName { get; set; } = null!;

    public string? TestCode { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public string? TurnaroundTime { get; set; }

    public string? SampleType { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<LabOrderDetail> LabOrderDetails { get; set; } = new List<LabOrderDetail>();
}
