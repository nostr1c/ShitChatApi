﻿using ShitChat.Infrastructure.Data;
using ShitChat.Domain.Entities;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Connections.DTOs;
using ShitChat.Application.Uploads.Services;

namespace ShitChat.Application.Users.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IUploadService _uploadService;

    public UserService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager,
        IUploadService uploadService
    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _uploadService = uploadService;
    }

    public async Task<(bool, string, UserDto?)> GetUserByGuidAsync(string userGuid)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userGuid);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Avatar = user.AvatarUri,
            CreatedAt = user.CreatedAt
        };

        return (true, "SuccessGotUser", userDto);
    }

    public async Task<(bool, string, string?)> UpdateAvatarAsync(IFormFile avatar)
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var (success, message, imageName) = await _uploadService.UploadFileAsync(avatar, 400, 400);
        if (!success || imageName == null)
            return (false, message, null);

        user.AvatarUri = imageName;
        await _userManager.UpdateAsync(user);

        return (true, "SuccessUpdatedAvatar", imageName);
    }

    public async Task<(bool, string, List<ConnectionDto>?)> GetConnectionsAsync()
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

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

        return (true, "SuccessGotUserConnections", connections);
    }

    public async Task<(bool, string, List<GroupDto>?)> GetUserGroupsAsync()
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var groups = await _dbContext.Groups
            .AsNoTracking()
            .Where(g => g.UserGroups.Any(ug => ug.UserId == userId))
            .OrderByDescending(g => g.LastActivity)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                LatestMessage = g.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),

                LastActivity = g.LastActivity,
                OwnerId = g.OwnerId,
                UnreadCount = g.Messages.Count(m =>
                    g.UserGroups.Any(ug => ug.UserId == userId) &&
                    (
                        g.UserGroups.First(ug => ug.UserId == userId).LastReadMessageId == null ||
                        m.CreatedAt > g.Messages
                            .Where(x => x.Id == g.UserGroups.First(ug => ug.UserId == userId).LastReadMessageId)
                            .Select(x => x.CreatedAt)
                            .FirstOrDefault()
                    )
                )
            })
            .ToListAsync();

        return (true, "SuccessGotUserGroups", groups);
    }
}