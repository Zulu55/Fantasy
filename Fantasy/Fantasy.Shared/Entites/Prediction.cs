using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entites;

public class Prediction
{
    public int Id { get; set; }

    public Tournament Tournament { get; set; } = null!;

    [Display(Name = "Tournament", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int TournamentId { get; set; }

    public Group Group { get; set; } = null!;

    [Display(Name = "Group", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int GroupId { get; set; }

    public Match Match { get; set; } = null!;

    [Display(Name = "Match", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int MatchId { get; set; }

    public User User { get; set; } = null!;

    [Display(Name = "User", ResourceType = typeof(Literals))]
    [MaxLength(450, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string UserId { get; set; } = null!;

    [Display(Name = "GoalsLocal", ResourceType = typeof(Literals))]
    public int? GoalsLocal { get; set; }

    [Display(Name = "GoalsVisitor", ResourceType = typeof(Literals))]
    public int? GoalsVisitor { get; set; }

    [Display(Name = "Points", ResourceType = typeof(Literals))]
    public int? Points { get; set; }
}