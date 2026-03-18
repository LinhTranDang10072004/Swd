using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int PrescriptionId { get; set; }

    public int PharmacistId { get; set; }

    public DateTime ExecutedAt { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Pharmacist Pharmacist { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
