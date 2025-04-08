namespace ShitChat.Application.DTOs;

public class InviteDto
{
    public string InviteString { get; set; }
    public UserDto Creator { get; set; } 
    public DateTime ValidThrough { get; set; }
}
