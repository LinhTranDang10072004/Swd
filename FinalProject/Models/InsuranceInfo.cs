using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class InsuranceInfo
{
    public int InsuranceId { get; set; }

    public string ProviderName { get; set; } = null!;

    public string? PolicyNumber { get; set; }

    public string? CoverageDetails { get; set; }

    public DateOnly? ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
