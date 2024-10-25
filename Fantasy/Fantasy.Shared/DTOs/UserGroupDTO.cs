using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.DTOs;

public class UserGroupDTO
{
    public int Id { get; set; }

    [MaxLength(450)]
    public string UserId { get; set; } = null!;

    public int GroupId { get; set; }

    public bool IsActive { get; set; }
}