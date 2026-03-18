using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CancellationLog> CancellationLogs { get; set; } = new List<CancellationLog>();

    public virtual Doctor? Doctor { get; set; }

    public virtual HospitalManager? HospitalManager { get; set; }

    public virtual ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();

    public virtual Patient? Patient { get; set; }

    public virtual Pharmacist? Pharmacist { get; set; }

    public virtual Receptionist? Receptionist { get; set; }

    public virtual ICollection<ReportExportLog> ReportExportLogs { get; set; } = new List<ReportExportLog>();

    public virtual ICollection<StaffSchedule> StaffSchedules { get; set; } = new List<StaffSchedule>();
}
