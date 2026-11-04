using System.ComponentModel.DataAnnotations;

namespace LUMS.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email hoặc MSSV / Mã nhân viên.")]
    [Display(Name = "Email hoặc MSSV / Mã nhân viên")]
    public string Identifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
