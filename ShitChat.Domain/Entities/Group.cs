using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string OwnerId { get; set; }

    // Navigation
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; }

    public List<User> Users { get; set; } = new List<User>();

    public ICollection<Message> Messages { get; set; }
}
