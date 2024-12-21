using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Models
{
    public class Group
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string OwnerId { get; set; }

        // Navigation
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
