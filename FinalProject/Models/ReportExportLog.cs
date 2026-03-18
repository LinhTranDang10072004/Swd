using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class ReportExportLog
{
    public int ExportId { get; set; }

    public int ReportId { get; set; }

    public int ExportedBy { get; set; }

    public DateTime ExportedAt { get; set; }

    public string Format { get; set; } = null!;

    public string? FileName { get; set; }

    public int? FileSizeKb { get; set; }

    public virtual User ExportedByNavigation { get; set; } = null!;

    public virtual RevenueReport Report { get; set; } = null!;
}
