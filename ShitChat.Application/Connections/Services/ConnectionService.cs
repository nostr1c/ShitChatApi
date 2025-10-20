using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Enums;
using ShitChat.Shared.Extensions;

namespace ShitChat.Application.Connections.Services;

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

    public async Task<(bool, ConnectionActionResult)> CreateConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.GetUserId();

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        if (friend == null)
            return (false, ConnectionActionResult.ErrorFriendNotFound);

        if (userId == friend.Id)
            return (false, ConnectionActionResult.ErrorCantAddYouself);

        var connectionExists = await _appDbContext.Connections
            .AnyAsync(c => c.UserId == userId && c.FriendId == friend.Id);

        if (connectionExists)
            return (false, ConnectionActionResult.ErrorConnectionAlreadyExists);

        var connection = new Connection
        {
            UserId = userId,
            FriendId = friendId
        };

        await _appDbContext.Connections.AddAsync(connection);
        await _appDbContext.SaveChangesAsync();

        return (true, ConnectionActionResult.SuccessCreatingConnection);
    }

    public async Task<(bool, ConnectionActionResult)> AcceptConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.GetUserId();

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        if (friend == null)
            return (false, ConnectionActionResult.ErrorFriendNotFound);

        var connection = await _appDbContext.Connections
            .FirstOrDefaultAsync(c => c.UserId == friendId && c.FriendId == userId);

        if (connection == null)
            return (false, ConnectionActionResult.ErrorFriendRequestNotFound);

        if (connection.Accepted)
            return (false, ConnectionActionResult.ErrorFriendRequestAlreadyAccepted);

        connection.Accepted = true;

        _appDbContext.Connections.Update(connection);

        await _appDbContext.SaveChangesAsync();

        return (true, ConnectionActionResult.SuccessAcceptingConnection);
    }

    public async Task<(bool, ConnectionActionResult)> DeleteConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.GetUserId();

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        var connection = await _appDbContext.Connections
               .FirstOrDefaultAsync(c =>
                    c.UserId == userId && c.FriendId == friendId ||
                    c.UserId == friendId && c.FriendId == userId);

        if (connection == null)
            return (false, ConnectionActionResult.ErrorFriendRequestNotFound);

        _appDbContext.Connections.Remove(connection);
        await _appDbContext.SaveChangesAsync();

        return (true, ConnectionActionResult.SuccessRemovingConnection);
    }
}
