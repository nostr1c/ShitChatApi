namespace ShitChat.Application.DTOs;

public class ConnectionDto
{
    public Guid Id { get; set; }
    public bool Accepted { get; set; }
    public UserDto User { get; set; }
    public DateTime CreatedAt { get; set; }
}
