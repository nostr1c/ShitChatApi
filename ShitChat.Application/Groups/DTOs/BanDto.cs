using ShitChat.Application.Users.DTOs;

namespace ShitChat.Application.Groups.DTOs;

public class BanDto
{
    public Guid Id { get; set; }
    public required UserDto BannedByUser { get; set; }
    public required UserDto UserDto { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
