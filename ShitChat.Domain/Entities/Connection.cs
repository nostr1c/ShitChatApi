using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShitChat.Domain.Entities;

public class Connection
{
    public Guid id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; }

    public string FriendId { get; set; }

    public bool Accepted { get; set; } = false;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation props
    [ForeignKey(nameof(UserId))]
    public User user { get; set; }

    [ForeignKey(nameof(FriendId))]
    public User friend { get; set; }
}