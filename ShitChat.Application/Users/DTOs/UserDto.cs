namespace ShitChat.Application.Users.DTOs;

public class UserDto
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? Avatar { get; set; }
    public DateOnly CreatedAt { get; set; }
}
