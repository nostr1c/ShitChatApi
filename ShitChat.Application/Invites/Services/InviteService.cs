using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShitChat.Application.Caching;
using ShitChat.Application.Users.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Extensions;
using System.Security.Cryptography;
using ShitChat.Application.Invites.Requests;
using ShitChat.Application.Invites.DTOs;
using ShitChat.Application.Caching.Services;
using ShitChat.Application.Groups.DTOs;

namespace ShitChat.Application.Invites.Services;

public class InviteService : IInviteService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cache;
    private readonly ILogger<InviteService> _logger;

    public InviteService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ICacheService cache,
        ILogger<InviteService> logger

    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
        _logger = logger;
    }
    public async Task<(bool, string, InviteDto?)> CreateInviteAsync(Guid groupGuid, CreateInviteRequest request)
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserGuid();
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser", null);

        var groupExists = await _dbContext.Groups.AnyAsync(x => x.Id == groupGuid);

        if (!groupExists)
            return (false, "ErrorGroupNotFound", null);

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
            Id = invite.Id,
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

    public async Task<(bool, string)> DeleteInviteAsync(Guid inviteId)
    {
        var invite = _dbContext.Invites.FirstOrDefault(x => x.Id == inviteId);
        if (invite == null) return (false, "ErrorInviteNotFound");

        _dbContext.Invites.Remove(invite);

        await _dbContext.SaveChangesAsync();

        return (true, "SuccessDeletedInvite");
    }

    public async Task<(bool, string, IEnumerable<InviteDto>?)> GetGroupInvites(Guid groupGuid)
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserGuid();
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser", null);

        var invites = await _dbContext.Invites
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Creator)
            .Where(x => x.GroupId == groupGuid).ToListAsync();

        var inviteDto = invites.Select(x => new InviteDto
        {
            Id = x.Id,
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
