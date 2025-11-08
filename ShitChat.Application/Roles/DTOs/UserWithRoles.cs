using ShitChat.Application.Users.DTOs;
using ShitChat.Domain.Entities;

namespace ShitChat.Application.Roles.DTOs;

public class UserWithRoles
{
    public required UserDto User { get; set; }
    public required IEnumerable<RoleDto> Roles { get; set; }
}