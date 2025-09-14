namespace ShitChat.Application.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    public bool IsFormerMember { get; set; }
}
