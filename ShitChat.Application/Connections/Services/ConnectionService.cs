using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShitChat.Application.Connections.DTOs;
using ShitChat.Application.Users.DTOs;
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

    public async Task<(bool, ConnectionActionResult, ConnectionActionDto?)> CreateConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.GetUserId();

        var user = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        if (friend == null)
            return (false, ConnectionActionResult.ErrorFriendNotFound, null);

        if (userId == friend.Id)
            return (false, ConnectionActionResult.ErrorCantAddYouself, null);

        var connectionExists = await _appDbContext.Connections
            .AnyAsync(c => (c.UserId == userId && c.FriendId == friend.Id) ||
                            (c.UserId == friendId && c.FriendId == userId));

        if (connectionExists)
            return (false, ConnectionActionResult.ErrorConnectionAlreadyExists, null);

        var connection = new Connection
        {
            UserId = userId,
            FriendId = friendId
        };
        var connectionDto = new ConnectionActionDto
        {
            ToReceiver = new ConnectionDto
            {
                Id = connection.id,
                Accepted = connection.Accepted,
                IsRequester = true,
                CreatedAt = connection.CreatedAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Avatar = user.AvatarUri,
                    Email = user.Email,
                    Username = user.UserName,
                    CreatedAt = user.CreatedAt,
                }
            },
            ToRequesterer = new ConnectionDto
            {
                Id = connection.id,
                Accepted = connection.Accepted,
                IsRequester = false,
                CreatedAt = connection.CreatedAt,
                User = new UserDto
                {
                    Id = friend.Id,
                    Avatar = friend.AvatarUri,
                    Email = friend.Email,
                    Username = friend.UserName,
                    CreatedAt = friend.CreatedAt,
                }
            }
        };

        await _appDbContext.Connections.AddAsync(connection);
        await _appDbContext.SaveChangesAsync();

        return (true, ConnectionActionResult.SuccessCreatingConnection, connectionDto);
    }

    public async Task<(bool, ConnectionActionResult, ConnectionActionDto?)> AcceptConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.GetUserId();

        var user = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        if (friend == null)
            return (false, ConnectionActionResult.ErrorFriendNotFound, null);

        var connection = await _appDbContext.Connections
            .FirstOrDefaultAsync(c => c.UserId == friendId && c.FriendId == userId);

        if (connection == null)
            return (false, ConnectionActionResult.ErrorFriendRequestNotFound, null);

        if (connection.Accepted)
            return (false, ConnectionActionResult.ErrorFriendRequestAlreadyAccepted, null);

        connection.Accepted = true;

        _appDbContext.Connections.Update(connection);

        await _appDbContext.SaveChangesAsync();

        var connectionDto = new ConnectionActionDto
        {
            ToReceiver = new ConnectionDto
            {
                Id = connection.id,
                Accepted = connection.Accepted,
                IsRequester = true,
                CreatedAt = connection.CreatedAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Avatar = user.AvatarUri,
                    Email = user.Email,
                    Username = user.UserName,
                    CreatedAt = user.CreatedAt,
                }
            },
            ToRequesterer = new ConnectionDto
            {
                Id = connection.id,
                Accepted = connection.Accepted,
                IsRequester = false,
                CreatedAt = connection.CreatedAt,
                User = new UserDto
                {
                    Id = friend.Id,
                    Avatar = friend.AvatarUri,
                    Email = friend.Email,
                    Username = friend.UserName,
                    CreatedAt = friend.CreatedAt,
                }
            }
        };

        return (true, ConnectionActionResult.SuccessAcceptingConnection, connectionDto);
    }

    public async Task<(bool, ConnectionActionResult, ConnectionActionDto?)> DeleteConnectionAsync(string friendId)
    {
        var userId = _httpContextAccessor.GetUserId();

        var user = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        var friend = await _appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == friendId);

        var connection = await _appDbContext.Connections
               .FirstOrDefaultAsync(c =>
                    c.UserId == userId && c.FriendId == friendId ||
                    c.UserId == friendId && c.FriendId == userId);

        if (connection == null)
            return (false, ConnectionActionResult.ErrorFriendRequestNotFound, null);

        _appDbContext.Connections.Remove(connection);
        await _appDbContext.SaveChangesAsync();

        var connectionDto = new ConnectionActionDto
        {
            ToReceiver = new ConnectionDto
            {
                Id = connection.id,
                Accepted = connection.Accepted,
                IsRequester = true,
                CreatedAt = connection.CreatedAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Avatar = user.AvatarUri,
                    Email = user.Email,
                    Username = user.UserName,
                    CreatedAt = user.CreatedAt,
                }
            },
            ToRequesterer = new ConnectionDto
            {
                Id = connection.id,
                Accepted = connection.Accepted,
                IsRequester = false,
                CreatedAt = connection.CreatedAt,
                User = new UserDto
                {
                    Id = friend.Id,
                    Avatar = friend.AvatarUri,
                    Email = friend.Email,
                    Username = friend.UserName,
                    CreatedAt = friend.CreatedAt,
                }
            }
        };

        return (true, ConnectionActionResult.SuccessRemovingConnection, connectionDto);
    }
}
