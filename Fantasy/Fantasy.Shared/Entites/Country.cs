using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entites;

public class Country
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;
}