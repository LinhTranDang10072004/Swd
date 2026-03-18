using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class StaffSchedule
{
    public int ScheduleId { get; set; }

    public int UserId { get; set; }

    public int ManagerId { get; set; }

    public string ShiftType { get; set; } = null!;

    public DateOnly ScheduleDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? Department { get; set; }

    public string Status { get; set; } = null!;

    public virtual HospitalManager Manager { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
