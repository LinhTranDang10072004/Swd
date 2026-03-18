using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class RevenueReport
{
    public int ReportId { get; set; }

    public int ManagerId { get; set; }

    public string? ReportName { get; set; }

    public string ReportType { get; set; } = null!;

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public decimal? TotalRevenue { get; set; }

    public decimal? TotalExpense { get; set; }

    public decimal? NetProfit { get; set; }

    public DateTime GeneratedAt { get; set; }

    public virtual HospitalManager Manager { get; set; } = null!;

    public virtual ICollection<ReportExportLog> ReportExportLogs { get; set; } = new List<ReportExportLog>();

    public virtual ICollection<RevenueReportItem> RevenueReportItems { get; set; } = new List<RevenueReportItem>();
}
