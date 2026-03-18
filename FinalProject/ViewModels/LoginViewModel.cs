using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Tên đăng nhập / Email")]
    public string UsernameOrEmail { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}

