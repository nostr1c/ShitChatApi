namespace ShitChat.Application.Groups.DTOs;

public class GroupDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? LatestMessage { get; set; }
    public int UnreadCount { get; set; }
    public required string OwnerId { get; set; }
    public DateTime? LastActivity { get; set; }
}
