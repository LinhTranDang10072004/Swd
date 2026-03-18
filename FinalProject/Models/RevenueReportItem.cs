using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class RevenueReportItem
{
    public int ItemId { get; set; }

    public int ReportId { get; set; }

    public string? Category { get; set; }

    public string? Department { get; set; }

    public int? ServiceCount { get; set; }

    public decimal Amount { get; set; }

    public decimal? Percentage { get; set; }

    public string? Description { get; set; }

    public virtual RevenueReport Report { get; set; } = null!;
}
