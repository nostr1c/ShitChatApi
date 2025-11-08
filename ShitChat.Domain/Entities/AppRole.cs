using Microsoft.AspNetCore.Identity;

namespace ShitChat.Domain.Entities;

public class AppRole : IdentityRole
{
    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}
