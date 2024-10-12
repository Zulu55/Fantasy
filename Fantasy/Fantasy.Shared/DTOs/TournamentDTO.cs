using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.DTOs;

public class TournamentDTO
{
    public int Id { get; set; }

    [Display(Name = "Team", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "Image", ResourceType = typeof(Literals))]
    public string? Image { get; set; }

    [Display(Name = "IsActive", ResourceType = typeof(Literals))]
    public bool IsActive { get; set; }

    [Display(Name = "Remarks", ResourceType = typeof(Literals))]
    public string? Remarks { get; set; }
}