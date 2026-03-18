using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class HospitalManager
{
    public int ManagerId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<ApprovalAction> ApprovalActions { get; set; } = new List<ApprovalAction>();

    public virtual ICollection<RevenueReport> RevenueReports { get; set; } = new List<RevenueReport>();

    public virtual ICollection<ServicePackagePriceHistory> ServicePackagePriceHistories { get; set; } = new List<ServicePackagePriceHistory>();

    public virtual ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();

    public virtual ICollection<StaffSchedule> StaffSchedules { get; set; } = new List<StaffSchedule>();

    public virtual User User { get; set; } = null!;
}
