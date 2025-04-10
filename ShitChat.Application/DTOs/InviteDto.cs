namespace ShitChat.Application.DTOs;

public class InviteDto
{
    public string InviteString { get; set; }
    public UserDto Creator { get; set; } 
    public DateOnly ValidThrough { get; set; }
}
