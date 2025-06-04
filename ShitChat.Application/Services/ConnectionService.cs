using ShitChat.Infrastructure.Data;
using ShitChat.Domain.Entities;
using ShitChat.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ShitChat.Application.Services;

public class ConnectionService : IConnectionService
{
    private UserManager<User> _userManager;
    private AppDbContext _appDbContext;

    public ConnectionService
    (
        UserManager<User> userManager,
        AppDbContext appDbContext
    )
    {
        _userManager = userManager;
        _appDbContext = appDbContext;
    }

    public async Task<(bool, string)> CreateConnectionAsync(string userName, string friendId)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
            return (false, "ErrorLoggedInUser");

        var friend = await _userManager.FindByIdAsync(friendId);
        if (friend == null)
            return (false, "ErrorFriendNotFound");

        if (user.Id == friend.Id)
            return (false, "ErrorCantAddYouself");

        var connectionExists = await _appDbContext.Connections
            .AnyAsync(c => c.UserId == user.Id && c.FriendId == friend.Id);

        if (connectionExists)
            return (false, "ErrorConnectionAlreadyExists");

        Connection connection = new Connection
        {
            UserId = user.Id,
            FriendId = friendId
        };

        await _appDbContext.Connections.AddAsync(connection);
        await _appDbContext.SaveChangesAsync();

        return (true, "SuccessCreatingConnection");
    }

    public async Task<(bool, string)> AcceptConnectionAsync(string userId, string friendId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "ErrorLoggedInUser");

        var friend = await _userManager.FindByIdAsync(friendId);
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

    public async Task<(bool, string)> DeleteConnectionAsync(string userId, string friendId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "ErrorLoggedInUser");

        var friend = await _userManager.FindByIdAsync(friendId);
        if (friend == null)
            return (false, "ErrorFriendNotFound");

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
