using ShitChat.Application.Roles.DTOs;

namespace ShitChat.Application.Users.Services;

public interface IUserRolesService
{
    Task<(bool, string, IEnumerable<RoleDto>?)> GetUserRoles(string userId);
    Task<(bool, string, UserWithRoles?)> AddRoleToUser(string userId, string roleId);
    Task<(bool, string)> RemoveRoleFromUser(string userId, string roleId);
}
