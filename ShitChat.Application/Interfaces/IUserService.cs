using ShitChat.Domain.Entities;
using ShitChat.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ShitChat.Application.Interfaces;

public interface IUserService
{
    public Task<(bool, User?)> GetUserByGuidAsync(string userGuid);
    public Task<(bool, string)> UpdateAvatarAsync(IFormFile avatar);
    public Task<List<ConnectionDto>> GetConnectionsAsync();
}
