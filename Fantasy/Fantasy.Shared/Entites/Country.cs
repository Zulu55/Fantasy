using System.ComponentModel.DataAnnotations;
using Fantasy.Shared.Resources;

namespace Fantasy.Shared.Entites;

public class Country
{
    public int Id { get; set; }

    [Display(Name = "Country", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    public ICollection<Team>? Teams { get; set; }

    public int TeamsCount => Teams == null ? 0 : Teams.Count;
}