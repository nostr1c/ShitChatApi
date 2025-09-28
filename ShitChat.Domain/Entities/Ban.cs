using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class Ban
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string? Reason { get; set; }

    // Foreign keys
    public string UserId { get; set; }
    public Guid GroupId { get; set; }
    public string BannedByUserId { get; set; }

    // Navigation props
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    [ForeignKey(nameof(GroupId))]
    public Group Group { get; set; } = null!;
    [ForeignKey(nameof(BannedByUserId))]
    public User BannedByUser { get; set; } = null!;

}
