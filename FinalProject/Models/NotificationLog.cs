using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class NotificationLog
{
    public int NotificationId { get; set; }

    public int? AppointmentId { get; set; }

    public int? UserId { get; set; }

    public int? PatientId { get; set; }

    public string Type { get; set; } = null!;

    public string Channel { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public int? RetryCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual User? User { get; set; }
}
