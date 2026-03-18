using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int InvoiceId { get; set; }

    public DateTime PaidAt { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal AmountPaid { get; set; }

    public decimal? ChangeAmount { get; set; }

    public string? TransactionRef { get; set; }

    public string? GatewayResponse { get; set; }

    public string Status { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;
}
