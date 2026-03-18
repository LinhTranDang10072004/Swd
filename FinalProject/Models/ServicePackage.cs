using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class ServicePackage
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public string Status { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual HospitalManager? CreatedByNavigation { get; set; }

    public virtual ICollection<ServicePackagePriceHistory> ServicePackagePriceHistories { get; set; } = new List<ServicePackagePriceHistory>();
}
