using Fantasy.Shared.Entites;
using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.DTOs;

public class UserDTO : User
{
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "LengthField", ErrorMessageResourceType = typeof(Literals))]
    public string Password { get; set; } = null!;

    [Compare("Password", ErrorMessageResourceName = "PasswordAndConfirmationDifferent", ErrorMessageResourceType = typeof(Literals))]
    [Display(Name = "PasswordConfirm", ResourceType = typeof(Literals))]
    [DataType(DataType.Password)]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "LengthField", ErrorMessageResourceType = typeof(Literals))]
    public string PasswordConfirm { get; set; } = null!;

    public string Language { get; set; } = null!;
}