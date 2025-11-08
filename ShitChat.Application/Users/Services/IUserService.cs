using Microsoft.AspNetCore.Http;
using ShitChat.Application.Connections.DTOs;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Roles.DTOs;
using ShitChat.Application.Users.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Users.Services;

public interface IUserService
{
    Task<(bool, UserActionResult, UserDto?)> GetUserByGuidAsync(string userGuid);
    Task<(bool, UserActionResult?, UploadActionResult?, string?)> UpdateAvatarAsync(IFormFile avatar);
    Task<(bool, UserActionResult, ConnectionsDto?)> GetConnectionsAsync();
    Task<(bool, UserActionResult, List<GroupDto>?)> GetUserGroupsAsync();
    Task<(bool, string, IEnumerable<UserWithRoles>)> GetUsersWithRolesAsync();
    Task<(bool, string?)> IndexAllUsersAsync();
    Task<(bool, string?, IReadOnlyCollection<UserDto>?)> SearchUsersAsync(string query);
}
