using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShitChat.Application.Interfaces;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Extensions;

namespace ShitChat.Application.Services;

public class ConnectionService : IConnectionService
{
    private AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ConnectionService
    (
        AppDbContext appDbContext,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _appDbContext = appDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool, string)> CreateConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser");

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        if (friend == null)
            return (false, "ErrorFriendNotFound");

        if (user.Id == friend.Id)
            return (false, "ErrorCantAddYouself");

        var connectionExists = await _appDbContext.Connections
            .AnyAsync(c => c.UserId == user.Id && c.FriendId == friend.Id);

        if (connectionExists)
            return (false, "ErrorConnectionAlreadyExists");

        var connection = new Connection
        {
            UserId = user.Id,
            FriendId = friendId
        };

        await _appDbContext.Connections.AddAsync(connection);
        await _appDbContext.SaveChangesAsync();

        return (true, "SuccessCreatingConnection");
    }

    public async Task<(bool, string)> AcceptConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser");

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        if (friend == null)
            return (false, "ErrorFriendNotFound");

        var connection = await _appDbContext.Connections
            .FirstOrDefaultAsync(c => c.UserId == friendId && c.FriendId == user.Id);

        if (connection == null)
            return (false, "ErrorFriendRequestNotFound");

        if (connection.Accepted)
            return (false, "ErrorFriendRequestAlreadyAccepted");

        connection.Accepted = true;

        _appDbContext.Connections.Update(connection);

        await _appDbContext.SaveChangesAsync();

        return (true, "SuccessAcceptingConnection");
    }

    public async Task<(bool, string)> DeleteConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser");

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        var connection = await _appDbContext.Connections
               .FirstOrDefaultAsync(c =>
                    (c.UserId == userId && c.FriendId == friendId) ||
                    (c.UserId == friendId && c.FriendId == userId));

        if (connection == null)
            return (false, "ErrorFriendRequestNotFound");

        _appDbContext.Connections.Remove(connection);
        await _appDbContext.SaveChangesAsync();

        return (true, "SuccessRemovingConnection");
    }
}
