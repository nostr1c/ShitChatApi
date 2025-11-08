namespace ShitChat.Application.Connections.DTOs;

public class ConnectionActionDto
{
    public ConnectionDto ToRequesterer { get; set; }
    public ConnectionDto ToReceiver { get; set; }
}
