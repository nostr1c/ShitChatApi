using ShitChat.Application.DTOs;
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

    public async Task<(bool, string, GroupDto?)> CreateGroupAsync(CreateGroupRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser", null);

        var group = new Group
        {
            Name = request.Name,
            OwnerId = userId
        };

        await _dbContext.Groups.AddAsync(group);
        await _dbContext.SaveChangesAsync();

        var userGroup = new UserGroup
        {
            UserId = user.Id,
            GroupId = group.Id,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.UserGroups.Add(userGroup);
        await _dbContext.SaveChangesAsync();

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = request.Name,
            OwnerId = userId
        };

        return (true, "SuccessCreatedGroup", groupDto);
    }
    
    public async Task<(bool, string, UserDto?)> AddUserToGroupAsync(Guid groupId, string userId)
    {
        var group = await _dbContext.Groups
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var exists = await _dbContext.UserGroups
            .AnyAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (exists)
            return (false, "ErrorUserAlreadyInGroup", null);

        var userGroup = new UserGroup
        {
            UserId = user.Id,
            GroupId = group.Id,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.UserGroups.Add(userGroup);
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

    public async Task<(bool, string)> KickUserFromGroupAsync(Guid groupId, string userId)
    {
        var group = await _dbContext.Groups
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound");

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound");

        var userGroup = await _dbContext.UserGroups
            .SingleOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null)
            return (false, "ErrorUserNotInGroup");

        var userRoles = await _dbContext.UserGroupRoles
            .Where(ugr => ugr.UserId == userId && ugr.GroupRole.GroupId == groupId)
            .ToListAsync();

        _dbContext.UserGroupRoles.RemoveRange(userRoles);
        await _dbContext.SaveChangesAsync();

        _dbContext.UserGroups.Remove(userGroup);
        await _dbContext.SaveChangesAsync();

        var cacheKey = CacheKeys.GroupMembers(group.Id);

        await _cache.KeyDeleteAsync(cacheKey);

        return (true, "SuccessKickedUser");
    }

    public async Task<(bool, string, GroupDto?)> GetGroupByGuidAsync(Guid groupId)
    {
        var group = await _dbContext.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            OwnerId = group.OwnerId
        };

        return (true, "SuccessGotGroup", groupDto);
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
            .Include(g => g.UserGroups)
                .ThenInclude(ug => ug.User)
                    .ThenInclude(u => u.GroupRoles)
                    .ThenInclude(gr =>  gr.GroupRole)
            .SingleOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var members = group.UserGroups.Select(x => new GroupMemberDto
        {
            User = new UserDto
            {
                Id = x.User.Id,
                Email = x.User.Email,
                Username = x.User.UserName,
                Avatar = x.User.AvatarUri,
                CreatedAt = x.User.CreatedAt
            },
            Roles = x.User.GroupRoles.Where(x => x.GroupRole.GroupId == groupId).Select(x => x.GroupRoleId).ToList()
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
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new MessageDto
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UserId = x.UserId,
                UserName = x.User.UserName,
                Avatar = x.User.AvatarUri,
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
            .Include(x => x.Roles)
                .ThenInclude(x => x.Permissions)
                    .ThenInclude(p => p.Permission)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupRolesNotFound", null);

        var roles = group.Roles
            .Select(x => new GroupRoleDto
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color,
                Permissions = x.Permissions
                    .Where(p => p.Permission != null)
                    .Select(p => p.Permission.Name)
                    .ToList()
            });

        return (true, "SuccessGotGroupRoles", roles);
    }

    public async Task<(bool, string, MessageDto?)> SendMessageAsync(Guid groupId, SendMessageRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var group = await _dbContext.Groups
            .AsNoTracking()
            .SingleOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

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

    public async Task<(bool, string, AddRoleToUserDto?)> AddRoleToUser(Guid groupId, string userId, Guid roleId)
    {
        var user = await _dbContext.Users
            .Include(u => u.GroupRoles)
            .SingleOrDefaultAsync(x => x.Id == userId );

        if (user == null)
            return (false, "ErrorUserNotFound", null);
        
        var role = await _dbContext.GroupRoles
            .SingleOrDefaultAsync(r => r.Id == roleId && r.GroupId == groupId);

        if (role == null)
            return (false, "ErrorRoleNotFound", null);

        if (user.GroupRoles.Any(x => x.GroupRoleId == role.Id))
            return (false, "ErrorUserAlreadyHasRole", null);

        user.GroupRoles.Add(new UserGroupRole
        {
            UserId = user.Id,
            GroupRoleId = role.Id
        });

        await _dbContext.SaveChangesAsync();

        await _cache.KeyDeleteAsync(CacheKeys.GroupMembers(groupId));

        var dto = new AddRoleToUserDto
        {
            GroupId = groupId,
            UserId = user.Id,
            RoleId = role.Id
        };

        return (true, "SuccessAddedRoleToUser", dto);
    }

    public async Task<(bool, string, RemoveRoleFromUserDto?)> RemoveRoleFromUser(Guid groupId, string userId, Guid roleId)
    {
        var user = await _dbContext.Users
            .Include(x => x.GroupRoles)
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var userGroupRole = user.GroupRoles.FirstOrDefault(x => x.GroupRoleId == roleId);
        if (userGroupRole == null)
            return (false, "ErrorRoleNotAssignedToUser", null);

        _dbContext.UserGroupRoles.Remove(userGroupRole);
        await _dbContext.SaveChangesAsync();

        await _cache.KeyDeleteAsync(CacheKeys.GroupMembers(groupId));

        var dto = new RemoveRoleFromUserDto
        {
            GroupId = groupId,
            UserId = user.Id,
            RoleId = roleId
        };

        return (true, "SuccessRemovedRoleFromUser", dto);
    }

    public async Task<(bool, string, GroupRoleDto?)> CreateRoleAsync(Guid groupId, CreateGroupRoleRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();

        if (userId == null)
            return (false, "ErrorLoggedInUser", null);

        var group = await _dbContext.Groups.SingleOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var groupRole = new GroupRole
        {
            GroupId = groupId,
            Name = request.Name,
            Color = request.Color,
            Permissions = request.Permissions
                .Select(pName => new GroupRolePermission
                {
                    PermissionId = _dbContext.Permissions
                        .Where(p => p.Name == pName)
                        .Select(p => p.Id)
                        .FirstOrDefault()
                })
                .ToList()
        };

        _dbContext.GroupRoles.Add(groupRole);
        await _dbContext.SaveChangesAsync();

        var groupRoleDto = new GroupRoleDto
        {
            Id = groupRole.Id,
            Name = groupRole.Name,
            Color = groupRole.Color,
            Permissions = groupRole.Permissions
                .Select(pr => _dbContext.Permissions
                    .Where(p => p.Id == pr.PermissionId)
                    .Select(p => p.Name)
                    .FirstOrDefault())
                .ToList()
        };

        return (true, "SuccessCreatedGroupRole", groupRoleDto);
    }

    public async Task<(bool, string, GroupRoleDto?)> EditRoleAsync(Guid roleId, EditGroupRoleRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();

        if (userId == null)
            return (false, "ErrorLoggedInUser", null);


        var groupRole = await _dbContext.GroupRoles
            .Include(x => x.Permissions)
                .ThenInclude(x => x.Permission)
            .SingleOrDefaultAsync(x => x.Id == roleId);

        if (groupRole == null)
            return (false, "ErrorRoleNotFound", null);

        groupRole.Name = request.Name;
        groupRole.Color = request.Color;

        _dbContext.GroupRolePermissions.RemoveRange(groupRole.Permissions);

        var permissions = await _dbContext.Permissions
            .Where(x => request.Permissions.Contains(x.Name))
            .ToListAsync();

        groupRole.Permissions = permissions.Select(p => new GroupRolePermission
            {
                GroupRoleId = groupRole.Id,
                PermissionId = p.Id
            })
            .ToList();

        await _dbContext.SaveChangesAsync();

        var groupRoleDto = new GroupRoleDto
        {
            Id = groupRole.Id,
            Name = groupRole.Name,
            Color = groupRole.Color,
            Permissions = permissions.Select(x => x.Name).ToList()
        };

        return (true, "SuccessEditedGroupRole", groupRoleDto);
    }

    public async Task<(bool, string)> MarkAsReadAsync(Guid groupId, MarkAsReadRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();

        if (userId == null)
            return (false, "ErrorLoggedInUser");

        var userGroup = await _dbContext.UserGroups
            .SingleOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null)
            return (false, "ErrorUserNotInGroup");

        userGroup.LastReadMessageId = request.LastMessageId;

        await _dbContext.SaveChangesAsync();

        return (true, "SuccessMarkedAsRead");
    }
}