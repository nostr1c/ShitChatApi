namespace ShitChat.Application.DTOs;

public class UserDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public DateOnly CreatedAt { get; set; }
}
