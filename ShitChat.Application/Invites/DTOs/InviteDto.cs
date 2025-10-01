using ShitChat.Application.Users.DTOs;

namespace ShitChat.Application.Invites.DTOs;

public class InviteDto
{
    public required Guid Id { get; set; }
    public required string InviteString { get; set; }
    public required UserDto Creator { get; set; } 
    public DateOnly ValidThrough { get; set; }
}
