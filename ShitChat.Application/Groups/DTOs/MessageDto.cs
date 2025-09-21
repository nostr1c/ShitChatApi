namespace ShitChat.Application.Groups.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public string? Avatar { get; set; }
    public bool IsFormerMember { get; set; }
}
