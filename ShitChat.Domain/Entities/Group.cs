using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string OwnerId { get; set; }

    // Navigation props
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; }

    public List<User> Users { get; set; } = new List<User>();

    public ICollection<Message> Messages { get; set; }
    public ICollection<GroupRole> Roles { get; set; }
    public ICollection<Invite> Invites { get; set; }
}
