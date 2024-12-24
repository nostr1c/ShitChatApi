using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; }
        public Guid GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
