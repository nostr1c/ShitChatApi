using Microsoft.AspNetCore.Identity;

namespace ShitChat.Domain.Entities;

public class AppUserRole : IdentityUserRole<string>
{
    public virtual User User { get; set; }
    public virtual AppRole Role { get; set; }
}
