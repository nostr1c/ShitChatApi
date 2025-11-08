using ShitChat.Application.Roles.DTOs;

namespace ShitChat.Application.Roles.Services;

public interface IRoleService
{
    Task<(bool, string, IEnumerable<RoleDto>)> GetRolesAsync();
    Task<(bool, string, RoleDto?)> CreateRoleAsync(string name);
    Task<(bool, string)> DeleteRoleAsync(string id);
}
