using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.DTOs;

public class ResetPasswordDTO
{
    [Display(Name = "Email", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [EmailAddress(ErrorMessageResourceName = "ValidEmail", ErrorMessageResourceType = typeof(Literals))]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "NewPassword", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "LengthField", ErrorMessageResourceType = typeof(Literals))]
    public string NewPassword { get; set; } = null!;

    [Compare("NewPassword", ErrorMessageResourceName = "PasswordAndConfirmationDifferent", ErrorMessageResourceType = typeof(Literals))]
    [Display(Name = "PasswordConfirm", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "LengthField", ErrorMessageResourceType = typeof(Literals))]
    public string ConfirmPassword { get; set; } = null!;

    public string Token { get; set; } = null!;
}