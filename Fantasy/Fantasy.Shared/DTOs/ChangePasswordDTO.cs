using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.DTOs;

public class ChangePasswordDTO
{
    [DataType(DataType.Password)]
    [Display(Name = "CurrentPassword", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "LengthField", ErrorMessageResourceType = typeof(Literals))]
    public string CurrentPassword { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "NewPassword", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "LengthField", ErrorMessageResourceType = typeof(Literals))]
    public string NewPassword { get; set; } = null!;

    [Compare("NewPassword", ErrorMessageResourceName = "PasswordAndConfirmationDifferent", ErrorMessageResourceType = typeof(Literals))]
    [Display(Name = "PasswordConfirm", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "LengthField", ErrorMessageResourceType = typeof(Literals))]
    public string Confirm { get; set; } = null!;
}