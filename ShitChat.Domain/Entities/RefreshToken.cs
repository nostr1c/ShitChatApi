namespace ShitChat.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }

    // Navigation props
    public User User { get; set; }
}
