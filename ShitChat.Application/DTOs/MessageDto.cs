namespace ShitChat.Application.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
}
