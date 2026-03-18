using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class ApprovalAction
{
    public int ActionId { get; set; }

    public int ManagerId { get; set; }

    public int OrderId { get; set; }

    public string Action { get; set; } = null!;

    public string? Reason { get; set; }

    public DateTime ActedAt { get; set; }

    public virtual HospitalManager Manager { get; set; } = null!;

    public virtual PurchaseOrder Order { get; set; } = null!;
}
