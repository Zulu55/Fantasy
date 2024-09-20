using Fantasy.Shared.Enums;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entites;

public class User : IdentityUser
{
    [Display(Name = "FirstName", ResourceType = typeof(Literals))]
    [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string FirstName { get; set; } = null!;

    [Display(Name = "LastName", ResourceType = typeof(Literals))]
    [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string LastName { get; set; } = null!;

    [Display(Name = "Image", ResourceType = typeof(Literals))]
    public string? Photo { get; set; }

    [Display(Name = "UserType", ResourceType = typeof(Literals))]
    public UserType UserType { get; set; }

    public Country Country { get; set; } = null!;

    [Display(Name = "Country", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int CountryId { get; set; }

    [Display(Name = "User", ResourceType = typeof(Literals))]
    public string FullName => $"{FirstName} {LastName}";
}