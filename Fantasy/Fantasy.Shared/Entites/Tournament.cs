using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entites;

public class Tournament
{
    public int Id { get; set; }

    [Display(Name = "Tournament", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    [Display(Name = "IsActive", ResourceType = typeof(Literals))]
    public bool IsActive { get; set; }

    [Display(Name = "Remarks", ResourceType = typeof(Literals))]
    public string? Remarks { get; set; }

    public string ImageFull => string.IsNullOrEmpty(Image) ? "/images/NoImage.png" : Image;

    public ICollection<TournamentTeam>? TournamentTeams { get; set; }

    public int TeamsCount => TournamentTeams == null ? 0 : TournamentTeams.Count;

    public ICollection<Match>? Matches { get; set; }

    public int MatchesCount => Matches == null ? 0 : Matches.Count;
}