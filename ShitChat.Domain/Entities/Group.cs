using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public DateTime? LastActivity { get; set; }

    // Foreign keys
    public string OwnerId { get; set; }

    // Navigation props
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; }

    public List<UserGroup> UserGroups { get; set; }

    public ICollection<Message> Messages { get; set; }
    public ICollection<GroupRole> Roles { get; set; }
    public ICollection<Invite> Invites { get; set; }
    public ICollection<Ban> Bans { get; set; }  
}
