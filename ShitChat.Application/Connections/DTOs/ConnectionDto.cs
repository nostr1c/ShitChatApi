using ShitChat.Application.Users.DTOs;

namespace ShitChat.Application.Connections.DTOs;

public class ConnectionDto
{
    public Guid Id { get; set; }
    public bool Accepted { get; set; }
    public bool IsRequester { get; set; }
    public required UserDto User { get; set; }
    public DateTime CreatedAt { get; set; }
}
