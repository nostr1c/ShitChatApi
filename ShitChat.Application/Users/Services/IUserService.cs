using ShitChat.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Connections.DTOs;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Users.Services;

public interface IUserService
{
    Task<(bool, UserActionResult, UserDto?)> GetUserByGuidAsync(string userGuid);
    Task<(bool, UserActionResult?, UploadActionResult?, string?)> UpdateAvatarAsync(IFormFile avatar);
    Task<(bool, UserActionResult, List<ConnectionDto>?)> GetConnectionsAsync();
    Task<(bool, UserActionResult, List<GroupDto>?)> GetUserGroupsAsync();
}
