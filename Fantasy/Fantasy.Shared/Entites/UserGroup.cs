using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entites;

public class UserGroup
{
    public int Id { get; set; }

    public User User { get; set; } = null!;

    [MaxLength(450)]
    public string UserId { get; set; } = null!;

    public Group Group { get; set; } = null!;

    public int GroupId { get; set; }

    public bool IsActive { get; set; }
}