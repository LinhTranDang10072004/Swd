using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class AuditSession
{
    public int SessionId { get; set; }

    public int PharmacistId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual ICollection<AuditDetail> AuditDetails { get; set; } = new List<AuditDetail>();

    public virtual Pharmacist Pharmacist { get; set; } = null!;
}
