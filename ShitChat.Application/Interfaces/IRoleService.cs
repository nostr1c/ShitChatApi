using ShitChat.Application.DTOs;
using ShitChat.Application.Requests;

namespace ShitChat.Application.Interfaces;

public interface IRoleService
{
    Task<(bool, string, GroupRoleDto?)> CreateRoleAsync(Guid groupId, CreateGroupRoleRequest request);
}
