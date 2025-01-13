using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.Data.Models;

public class Connection
{
    public Guid id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; }

    public string FriendId { get; set; }

    public bool Accepted { get; set; } = false;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User user { get; set; }

    [ForeignKey(nameof(FriendId))]
    public User friend { get; set; }
}