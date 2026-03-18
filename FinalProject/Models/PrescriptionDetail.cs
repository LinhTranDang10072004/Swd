using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class PrescriptionDetail
{
    public int DetailId { get; set; }

    public int PrescriptionId { get; set; }

    public int MedicineId { get; set; }

    public string? Dosage { get; set; }

    public string? Frequency { get; set; }

    public string? Duration { get; set; }

    public int Quantity { get; set; }

    public decimal? MorningDose { get; set; }

    public decimal? NoonDose { get; set; }

    public decimal? EveningDose { get; set; }

    public string? Instructions { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
