﻿using ShitChat.Application.DTOs;
using ShitChat.Application.Requests;
using ShitChat.Application.Interfaces;
using ShitChat.Infrastructure.Data;
using ShitChat.Domain.Entities;
using ShitChat.Shared.Extensions;
using ShitChat.Application.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ShitChat.Application.Services;

public class GroupService : IGroupService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GroupService> _logger;
    private readonly ICacheService _cache;

    public GroupService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<GroupService> logger,
        ICacheService cache
    )
    { 
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _cache = cache;
    }

    public async Task<(bool, string, UserDto?)> AddUserToGroupAsync(Guid groupId, string userId)
    {
        var group = await _dbContext.Groups
            .Include(x => x.Users)
            .SingleOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            return (false, "ErrorUserNotFound", null);

        if (group.Users.Any(x => x.Id == user.Id))
            return (false, "ErrorUserAlreadyInGroup", null);

        group.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        var dto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            Avatar = user.AvatarUri,
            CreatedAt = user.CreatedAt
        };

        return (true, "SuccessAddedUserToGroup", dto);
    }

    public async Task<GroupDto> CreateGroupAsync(CreateGroupRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        var group = new Group
        {
            Name = request.Name,
            OwnerId = userId
        };

        group.Users.Add(user);

        await _dbContext.Groups.AddAsync(group);
        await _dbContext.SaveChangesAsync();

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = request.Name,
            OwnerId = userId
        };

        return groupDto;
    }

    public async Task<GroupDto?> GetGroupByGuidAsync(Guid groupId)
    {
        var group = await _dbContext.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null) return null;

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            OwnerId = group.OwnerId
        };

        return groupDto;
    }
     
    public async Task<(bool, string, IEnumerable<GroupMemberDto>?)> GetGroupMembersAsync(Guid groupId)
    {
        var cacheKey = CacheKeys.GroupMembers(groupId);

        var cached = await _cache.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            var cachedMembers = JsonSerializer.Deserialize<IEnumerable<GroupMemberDto>>(cached);
            return (true, "SuccessGotGroupMembers", cachedMembers);
        }
        
        var group = await _dbContext.Groups
            .AsNoTracking()
            .Include(x => x.Users)
            .ThenInclude(y => y.GroupRoles)
            .ThenInclude(z =>  z.GroupRole)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var members = group.Users.Select(x => new GroupMemberDto
        {
            User = new UserDto
            {
                Id = x.Id,
                Email = x.Email,
                Username = x.UserName,
                Avatar = x.AvatarUri,
                CreatedAt = x.CreatedAt
            },
            Roles = x.GroupRoles.Where(y => y.GroupRole.GroupId == groupId).Select(x => new GroupRoleDto
            {
                Id = x.GroupRoleId,
                Name = x.GroupRole.Name,
                Color = x.GroupRole.Color
            })
        });

        var json = JsonSerializer.Serialize(members);
        await _cache.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(10));

        return (true, "SuccessGotGroupMembers", members);
    }

    public async Task<(bool, string, IEnumerable<MessageDto>?)> GetGroupMessagesAsync(Guid groupGuid, Guid? lastMessageId, int take)
    {
        var cacheKey = CacheKeys.GroupMessages(groupGuid);

        if (lastMessageId == null)
        {
            var cached = await _cache.StringGetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedMessages = JsonSerializer.Deserialize<List<MessageDto>>(cached)!;
                return (true, "SuccessGotGroupMessages", cachedMessages);
            }
        }

        var query = _dbContext.Messages
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Group)
            .AsNoTracking()
            .Where(x => x.GroupId == groupGuid);

        if (lastMessageId != null)
        {
            var lastMessage = await _dbContext.Messages.FindAsync(lastMessageId);
            if (lastMessage != null)
            {
                query = query.Where(x => x.CreatedAt < lastMessage.CreatedAt).OrderByDescending(x => x.CreatedAt);
            }
        }

        var messages = await query
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new MessageDto
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UserId = x.UserId
            })
            .ToListAsync();

        if (lastMessageId == null)
        {
            var json = JsonSerializer.Serialize(messages);
            await _cache.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));
        }

        return (true, "SuccessGotGroupMessages", messages);
    }

    public async Task<(bool, string, IEnumerable<GroupRoleDto>?)> GetGroupRolesAsync(Guid groupId)
    {
        var group = await _dbContext.Groups
            .AsNoTracking()
            .Include (x => x.Roles)
            .AsNoTracking()
            .SingleOrDefaultAsync (x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupRolesNotFound", null);

        var roles = group.Roles
            .Select(x => new GroupRoleDto
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color
            });

        return (true, "SuccessGotGroupRoles", roles);
    }

    public async Task<List<GroupDto>> GetUserGroupsAsync()
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();

        var groups = await _dbContext.Groups
            .AsNoTracking()
            .Where(x => x.Users.Any(x => x.Id == userId))
            .Select(x => new GroupDto
            {
                Id = x.Id,
                Name = x.Name,
                Latest = x.Messages
                    .OrderByDescending(y => y.CreatedAt)
                    .Select(z => z.Content)
                    .FirstOrDefault(),
                OwnerId = x.OwnerId,
            }).ToListAsync();

        return groups;
    }

    public async Task<(bool, string, MessageDto?)> SendMessageAsync(Guid groupId, SendMessageRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var group = await _dbContext.Groups
            .Include(g => g.Users)
            .AsNoTracking()
            .SingleOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        if (!group.Users.Any(u => u.Id == userId))
            return (false, "ErrorUserNotInGroup", null);

        var message = new Message
        {
            Content = request.Content,
            UserId = user.Id,
            GroupId = groupId,
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        var messageDto = new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UserId = message.UserId
        };

        var cacheKey = CacheKeys.GroupMessages(groupId);

        var cached = await _cache.StringGetAsync(cacheKey);
        List<MessageDto> list;
        if (!string.IsNullOrEmpty(cached))
        {
            list = JsonSerializer.Deserialize<List<MessageDto>>(cached)!
                .Take(39)
                .ToList();
        }
        else
        {
            list = new List<MessageDto>();
        }

        list.Insert(0, messageDto);

        var updatedJson = JsonSerializer.Serialize(list);
        await _cache.StringSetAsync(cacheKey, updatedJson, TimeSpan.FromMinutes(5));

        return (true, "SuccessSentMessage",  messageDto);
    }
}
