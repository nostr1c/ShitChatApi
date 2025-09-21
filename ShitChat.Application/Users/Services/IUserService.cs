using ShitChat.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Connections.DTOs;

namespace ShitChat.Application.Users.Services;

public interface IUserService
{
    Task<(bool, string, UserDto?)> GetUserByGuidAsync(string userGuid);
    Task<(bool, string, string?)> UpdateAvatarAsync(IFormFile avatar);
    Task<(bool, string, List<ConnectionDto>?)> GetConnectionsAsync();
    Task<(bool, string, List<GroupDto>?)> GetUserGroupsAsync();
}
