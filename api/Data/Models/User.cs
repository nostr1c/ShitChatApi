using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace api.Data.Models;

public class User : IdentityUser
{
    public string? AvatarUri { get; set; }

    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    // Navigation
    public ICollection<Connection> Connections { get; set; }

    public ICollection<Group> OwnedGroups { get; set; }

    public ICollection<Group> Groups { get; set; }
}
