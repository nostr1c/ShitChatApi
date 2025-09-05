namespace ShitChat.Domain.Entities;

public class UserGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? LastReadMessageId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public string UserId { get; set; }
    public Guid GroupId { get; set; }

    // Navigation props
    public User User { get; set; }
    public Group Group { get; set; }
}
