using ShitChat.Domain.Entities;
using ShitChat.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ShitChat.Application.Interfaces;

public interface IUserService
{
    Task<(bool, string, UserDto?)> GetUserByGuidAsync(string userGuid);
    Task<(bool, string, string?)> UpdateAvatarAsync(IFormFile avatar);
    Task<(bool, string, List<ConnectionDto>)> GetConnectionsAsync();
    Task<(bool, string, List<GroupDto>)> GetUserGroupsAsync();
}
