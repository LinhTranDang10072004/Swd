using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int PatientId { get; set; }

    public int? ReceptionistId { get; set; }

    public int? AppointmentId { get; set; }

    public DateTime IssuedAt { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal FinalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Receptionist? Receptionist { get; set; }
}
