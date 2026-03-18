using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class AuditDetail
{
    public int DetailId { get; set; }

    public int SessionId { get; set; }

    public int InventoryId { get; set; }

    public int BookQuantity { get; set; }

    public int? PhysicalCount { get; set; }

    public int? Variance { get; set; }

    public string? VarianceReason { get; set; }

    public bool? IsResolved { get; set; }

    public virtual Inventory Inventory { get; set; } = null!;

    public virtual AuditSession Session { get; set; } = null!;
}
