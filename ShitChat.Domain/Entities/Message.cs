using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; }
    public Guid GroupId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(GroupId))]
    public Group Group { get; set; }


    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

}
