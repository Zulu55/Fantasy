using Fantasy.Shared.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<TournamentTeam> TournamentTeams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Team>().HasIndex(x => new { x.CountryId, x.Name }).IsUnique();
        modelBuilder.Entity<Tournament>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<TournamentTeam>().HasIndex(x => new { x.TournamentId, x.TeamId }).IsUnique();
        DisableCascadingDelete(modelBuilder);
    }

    private void DisableCascadingDelete(ModelBuilder modelBuilder)
    {
        var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
        foreach (var relationship in relationships)
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}