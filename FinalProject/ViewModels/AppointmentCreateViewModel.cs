using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProject.ViewModels;

public class AppointmentCreateViewModel
{
    [Required]
    public int PatientId { get; set; }

    public string? PatientFullName { get; set; }
    public string? PatientNationalId { get; set; }

    [Required]
    [Display(Name = "Khoa/Chuyên khoa")]
    public int SpecialtyId { get; set; }

    [Required]
    [Display(Name = "Bác sĩ")]
    public int DoctorId { get; set; }

    [Required]
    [Display(Name = "Thời gian hẹn")]
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; } = DateTime.Now.AddHours(1);

    [Display(Name = "Lý do khám")]
    public string? Reason { get; set; }

    public List<SelectListItem> Specialties { get; set; } = new();
    public List<SelectListItem> Doctors { get; set; } = new();
}

