using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels;

public class PatientRegistrationViewModel
{
    [Required]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = "";

    [Display(Name = "CMND/CCCD")]
    public string? NationalId { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public string? Gender { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [Display(Name = "Email")]
    [EmailAddress]
    public string? Email { get; set; }

    public string? ReturnUrl { get; set; }
}

