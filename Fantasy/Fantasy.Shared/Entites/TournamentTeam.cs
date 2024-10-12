namespace Fantasy.Shared.Entites;

public class TournamentTeam
{
    public int Id { get; set; }

    public Tournament Tournament { get; set; } = null!;

    public int TournamentId { get; set; }

    public Team Team { get; set; } = null!;

    public int TeamId { get; set; }
}