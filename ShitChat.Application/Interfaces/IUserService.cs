using Domain.Entities;
using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IUserService
{
    public Task<(bool, User?)> GetUserByGuidAsync(string userGuid);
    public Task<(bool, string)> UpdateAvatarAsync(IFormFile avatar);
    public Task<List<ConnectionDto>> GetConnectionsAsync();
}
