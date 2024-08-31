using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entites;

public class Team
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public Country? Country { get; set; }

    public int CountryId { get; set; }
}