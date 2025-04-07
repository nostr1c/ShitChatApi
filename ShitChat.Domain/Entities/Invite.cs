using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class Invite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ValidThrough { get; set; }
    public string UserId { get; set; }
    public Guid GroupId { get; set; }

    // Navigation props
    [ForeignKey(nameof(UserId))]
    public User Creator { get; set; }

    [ForeignKey(nameof(GroupId))]
    public Group Group { get; set; }

}
