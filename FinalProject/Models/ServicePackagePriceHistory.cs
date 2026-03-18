using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class ServicePackagePriceHistory
{
    public int HistoryId { get; set; }

    public int PackageId { get; set; }

    public decimal? OldPrice { get; set; }

    public decimal NewPrice { get; set; }

    public int ChangedBy { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? Reason { get; set; }

    public virtual HospitalManager ChangedByNavigation { get; set; } = null!;

    public virtual ServicePackage Package { get; set; } = null!;
}
