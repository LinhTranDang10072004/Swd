using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Receptionist
{
    public int ReceptionistId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User User { get; set; } = null!;
}
