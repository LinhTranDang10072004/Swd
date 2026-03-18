using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class CancellationLog
{
    public int LogId { get; set; }

    public int AppointmentId { get; set; }

    public int CancelledBy { get; set; }

    public DateTime CancelledAt { get; set; }

    public string? Reason { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual User CancelledByNavigation { get; set; } = null!;
}
