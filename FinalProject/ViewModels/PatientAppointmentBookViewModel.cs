using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProject.ViewModels;

public class PatientAppointmentBookViewModel
{
    public int? PreselectedDoctorId { get; set; }

    [Required]
    [Display(Name = "Khoa/Chuyên khoa")]
    public int SpecialtyId { get; set; }

    [Required]
    [Display(Name = "Bác sĩ")]
    public int DoctorId { get; set; }

    [Required]
    [Display(Name = "Thời gian hẹn")]
    public DateTime AppointmentDate { get; set; }

    [Display(Name = "Lý do khám")]
    public string? Reason { get; set; }

    public List<SelectListItem> Specialties { get; set; } = new();
    public List<SelectListItem> Doctors { get; set; } = new();
}

