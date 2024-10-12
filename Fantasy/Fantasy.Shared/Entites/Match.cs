using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entites;

public class Match
{
    public int Id { get; set; }

    public Tournament Tournament { get; set; } = null!;

    [Display(Name = "Tournament", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int TournamentId { get; set; }

    [Display(Name = "Date", ResourceType = typeof(Literals))]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
    public DateTime Date { get; set; }

    [Display(Name = "IsActive", ResourceType = typeof(Literals))]
    public bool IsActive { get; set; }

    public Team Local { get; set; } = null!;

    [Display(Name = "Local", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int LocalId { get; set; }

    public Team Visitor { get; set; } = null!;

    [Display(Name = "Visitor", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int VisitorId { get; set; }

    [Display(Name = "GoalsLocal", ResourceType = typeof(Literals))]
    public int? GoalsLocal { get; set; }

    [Display(Name = "GoalsVisitor", ResourceType = typeof(Literals))]
    public int? GoalsVisitor { get; set; }

    [Display(Name = "Date", ResourceType = typeof(Literals))]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
    public DateTime DateLocal => Date.ToLocalTime();

    public bool IsClosed { get; set; }
}