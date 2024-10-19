using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entites;

public class Group
{
    public int Id { get; set; }

    [Display(Name = "Group", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    public User Admin { get; set; } = null!;

    [Display(Name = "Admin", ResourceType = typeof(Literals))]
    [MaxLength(450, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string AdminId { get; set; } = null!;

    public Tournament Tournament { get; set; } = null!;

    [Display(Name = "Tournament", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int TournamentId { get; set; }

    [Display(Name = "Code", ResourceType = typeof(Literals))]
    [MaxLength(6, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Code { get; set; } = null!;

    public string? Image { get; set; }

    [Display(Name = "IsActive", ResourceType = typeof(Literals))]
    public bool IsActive { get; set; }

    [Display(Name = "Remarks", ResourceType = typeof(Literals))]
    public string? Remarks { get; set; }

    public ICollection<UserGroup>? Members { get; set; }

    public string ImageFull => string.IsNullOrEmpty(Image) ? "/images/NoImage.png" : Image;
}