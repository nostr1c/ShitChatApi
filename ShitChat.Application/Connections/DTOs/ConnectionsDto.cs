namespace ShitChat.Application.Connections.DTOs;

public class ConnectionsDto
{
    public IEnumerable<ConnectionDto> SentRequests { get; set; }
    public IEnumerable<ConnectionDto> ReceivedRequests { get; set; }
    public IEnumerable<ConnectionDto> Accepted { get; set; }
}
