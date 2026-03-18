using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels;

public class AppointmentLookupViewModel
{
    [Required]
    [Display(Name = "Nhập CMND/CCCD")]
    public string NationalId { get; set; } = "";
}

