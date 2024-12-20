using Microsoft.AspNetCore.Identity;

namespace api.Data.Models
{
    public class User : IdentityUser
    {
        public string? AvatarUri { get; set; }

        public ICollection<Connection> Connections { get; set; }
    }
}
