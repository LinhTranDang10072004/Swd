using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string? RoomType { get; set; }

    public string? Floor { get; set; }

    public string GenderAllowed { get; set; } = null!;

    public int TotalBeds { get; set; }

    public int AvailableBeds { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<HospitalAdmissionRequest> HospitalAdmissionRequests { get; set; } = new List<HospitalAdmissionRequest>();
}
