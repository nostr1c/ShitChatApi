using Microsoft.AspNetCore.Identity;

namespace ShitChat.Domain.Entities;

public class User : IdentityUser
{
    public string? AvatarUri { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);


    // Navigation props
    public ICollection<Connection> Connections { get; set; }

    public ICollection<Group> OwnedGroups { get; set; }

    public ICollection<Group> Groups { get; set; }
}
