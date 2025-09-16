namespace ShitChat.Application.DTOs;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? LatestMessage { get; set; }
    public int UnreadCount { get; set; }
    public string OwnerId { get; set; }
    public DateTime? LastActivity { get; set; }
}
