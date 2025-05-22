using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using ShitChat.Application.Requests;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Extensions;
using System.Security.Cryptography;

namespace ShitChat.Application.Services;

public class InviteService : IInviteService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<InviteService> _logger;

    public InviteService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<InviteService> logger

    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    public async Task<(bool, string, InviteDto?)> CreateInviteAsync(Guid groupGuid, CreateInviteRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return (false, "ErrorLoggedInUser", null);
        }

        var groupExists = await _dbContext.Groups.AnyAsync(x => x.Id == groupGuid);

        if (!groupExists)
        {
            return (false, "ErrorGroupNotFound", null);
        }

        var invite = new Invite
        {
            GroupId = groupGuid,
            UserId = userId,
            ValidThrough = request.ValidThrough,
            InviteString = GenerateInviteString()
        };

        _dbContext.Invites.Add(invite);
        await _dbContext.SaveChangesAsync();


        var inviteDto = new InviteDto
        {
            Creator = new UserDto
            {
                Id = user.Id,
                Avatar = user.AvatarUri,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
                Username = user.UserName
            },
            ValidThrough = invite.ValidThrough,
            InviteString = invite.InviteString
        };


        return (true, "SuccessCreatedInvite", inviteDto);
    }

    public async Task<(bool, string, IEnumerable<InviteDto>?)> GetGroupInvites(Guid groupGuid)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return (false, "ErrorLoggedInUser", null);
        }

        var invites = await _dbContext.Invites
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Creator)
            .Where(x => x.GroupId == groupGuid).ToListAsync();

        var inviteDto = invites.Select(x => new InviteDto
        {
            Creator = new UserDto
            {
                Id = x.Creator.Id,
                Avatar = x.Creator.AvatarUri,
                CreatedAt = x.Creator.CreatedAt,
                Email = x.Creator.Email,
                Username = x.Creator.UserName
            },
            InviteString = x.InviteString,
            ValidThrough = x.ValidThrough,
        });
        
        return (true, "SuccessGotGroupInvites", inviteDto);
    }

    public async Task<(bool, string, JoinInviteDto?)> JoinWithInviteAsync(string inviteString)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users
            .Include(u => u.Groups)
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return (false, "ErrorLoggedInUser", null);
        }

        var invite = await _dbContext.Invites
            .Include(i => i.Group)
            .ThenInclude(g => g.Users)
            .SingleOrDefaultAsync(x => x.InviteString == inviteString);

        if (invite == null)
        {
            return (false, "ErrorInviteNotFound", null);
        }

        if (invite.ValidThrough < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return (false, "ErrorInviteExpired", null);
        }

        var group = invite.Group;

        var joinInviteDto = new JoinInviteDto
        {
            Group = group.Id
        };

        if (group.Users.Any(x => x.Id == user.Id))
        {
            return (false, "ErrorAlreadyInGroup", joinInviteDto);
        }

        group.Users.Add(user);

        await _dbContext.SaveChangesAsync();


        return (true, "SuccessJoinedGroup", joinInviteDto);
    }


    private string GenerateInviteString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var data = new byte[8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(data);

        var result = new char[8];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = chars[data[i] % chars.Length];
        }

        return new string(result);
    }
}
