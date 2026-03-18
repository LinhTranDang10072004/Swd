using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int UserId { get; set; }

    public int SpecialtyId { get; set; }

    public string? LicenseNumber { get; set; }

    public int? YearsExperience { get; set; }

    public string? Department { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<HospitalAdmissionRequest> HospitalAdmissionRequests { get; set; } = new List<HospitalAdmissionRequest>();

    public virtual ICollection<LabOrderDetail> LabOrderDetails { get; set; } = new List<LabOrderDetail>();

    public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual Specialty Specialty { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
