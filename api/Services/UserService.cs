using api.Data;
using api.Data.Models;
using api.Extensions;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool, User?)> GetUserByGuidAsync(string userGuid)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userGuid);

        if (user == null)
            return (false, null);

        return (true, user);
    }

    public async Task<(bool, string?)> UpdateAvatarAsync(UpdateAvatarRequest request)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == _httpContextAccessor.HttpContext.User.GetUserGuid());

        if (user == null)
            return (false, null);

        user.AvatarUri = request.AvatarUri;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return (true, request.AvatarUri);
    }

    public async Task<List<ConnectionDto>> GetConnectionsAsync()
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();

        var connections = await _dbContext.Connections
                .Include(x => x.user)
                .Include(x => x.friend)
                .Where(x => x.UserId == userId || x.FriendId == userId)
                .Select(x => new ConnectionDto
                {
                    Id = x.id,
                    Accepted = x.Accepted,
                    CreatedAt = x.CreatedAt,
                    User = x.UserId == userId
                        ? new UserDto
                        {
                            Id = x.friend.Id,
                            Username = x.friend.UserName,
                            Email = x.friend.Email,
                            Avatar = x.friend.AvatarUri
                        }
                        : new UserDto
                        {
                            Id = x.user.Id,
                            Username = x.user.UserName,
                            Email = x.user.Email,
                            Avatar = x.user.AvatarUri
                        }
                })
                .ToListAsync();

        return connections;
    }
}
