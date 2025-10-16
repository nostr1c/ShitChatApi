using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShitChat.Application.Caching;
using ShitChat.Application.Caching.Services;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Groups.Requests;
using ShitChat.Application.Invites.DTOs;
using ShitChat.Application.Uploads.Services;
using ShitChat.Application.Users.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Extensions;
using System.Text.Json;

namespace ShitChat.Application.Groups.Services;

public class GroupService : IGroupService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GroupService> _logger;
    private readonly ICacheService _cache;
    private readonly IUploadService _uploadService;

    public GroupService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<GroupService> logger,
        ICacheService cache,
        IUploadService uploadService
    )
    { 
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _cache = cache;
        _uploadService = uploadService;
    }

    public async Task<(bool, string, GroupDto?)> CreateGroupAsync(CreateGroupRequest request)
    {
        var userId = _httpContextAccessor.GetUserId();

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var group = new Group
        {
            Name = request.Name,
            OwnerId = userId
        };

        _dbContext.Groups.Add(group);

        var userGroup = new UserGroup
        {
            UserId = userId,
            GroupId = group.Id,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.UserGroups.Add(userGroup);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

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
        var exists = await _dbContext.UserGroups
            .AsNoTracking()
            .AnyAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (exists)
            return (false, "ErrorUserAlreadyInGroup", null);

        var groupExists = await _dbContext.Groups
            .AsNoTracking()
            .AnyAsync(g => g.Id == groupId);

        if (!groupExists)
            return (false, "ErrorGroupNotFound", null);

        var user = await _dbContext.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.UserName,
                Avatar = u.AvatarUri,
                CreatedAt = u.CreatedAt
            }).SingleOrDefaultAsync();

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var userGroup = new UserGroup
        {
            UserId = user.Id,
            GroupId = groupId,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.UserGroups.Add(userGroup);
        await _dbContext.SaveChangesAsync();

        return (true, "SuccessAddedUserToGroup", user);
    }

    public async Task<(bool, string)> KickUserFromGroupAsync(Guid groupId, string userId)
    {
        var userGroup = await _dbContext.UserGroups
            .SingleOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null)
            return (false, "ErrorUserNotInGroup");

        // TODO: Cascade this later?

        var userRoles = await _dbContext.UserGroupRoles
            .Where(ugr => ugr.UserId == userId && ugr.GroupRole.GroupId == groupId)
            .ToListAsync();

        _dbContext.UserGroupRoles.RemoveRange(userRoles);
        _dbContext.UserGroups.Remove(userGroup);
        await _dbContext.SaveChangesAsync();

        await _cache.KeyDeleteAsync(CacheKeys.GroupMembers(groupId));
        return (true, "SuccessKickedUser");
    }

    public async Task<(bool, string)> BanUserFromGroupAsync(Guid groupId, string userId, BanUserRequest request)
    {
        var userGroup = await _dbContext.UserGroups
            .SingleOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null)
            return (false, "ErrorUserNotInGroup");

        // TODO: Cascade this later?
        var userRoles = await _dbContext.UserGroupRoles
            .Where(ugr => ugr.UserId == userId && ugr.GroupRole.GroupId == groupId)
            .ToListAsync();

        _dbContext.UserGroupRoles.RemoveRange(userRoles);
        _dbContext.UserGroups.Remove(userGroup);

        var ban = new Ban
        {
            UserId = userId,
            GroupId = groupId,
            CreatedAt = DateTime.UtcNow,
            Reason = request.Reason,
            BannedByUserId = _httpContextAccessor.GetUserId()

        };
        _dbContext.Bans.Add(ban);

        await _dbContext.SaveChangesAsync();
        await _cache.KeyDeleteAsync(CacheKeys.GroupMembers(groupId));

        return (true, "SuccessBannedUser");
    }

    public async Task<(bool, string, GroupDto?)> GetGroupByGuidAsync(Guid groupId)
    {
        var groupDto = await _dbContext.Groups
            .AsNoTracking()
            .Select(x => new GroupDto
            {
                Id = groupId,
                Name = x.Name,
                OwnerId = x.OwnerId,
            })
            .FirstOrDefaultAsync(x => x.Id == groupId);

        if (groupDto == null)
            return (false, "ErrorGroupNotFound", null);

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
        
        var groupExists = await _dbContext.Groups
            .AsNoTracking()
            .AnyAsync(x => x.Id == groupId);

        if (!groupExists)
            return (false, "ErrorGroupNotFound", null);

        var members = await _dbContext.UserGroups
            .AsNoTracking()
            .Where(ug => ug.GroupId == groupId)
            .Select(ug => new GroupMemberDto
            {
                User = new UserDto
                {
                    Id = ug.User.Id,
                    Email = ug.User.Email,
                    Username = ug.User.UserName,
                    Avatar = ug.User.AvatarUri,
                    CreatedAt = ug.User.CreatedAt
                },
                Roles = ug.User.GroupRoles
                    .Where(gr => gr.GroupRole.GroupId == groupId)
                    .Select(gr => gr.GroupRoleId)
                    .ToList()
            }).ToListAsync();

        var json = JsonSerializer.Serialize(members);
        await _cache.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(10));

        return (true, "SuccessGotGroupMembers", members);
    }

    public async Task<(bool, string, IEnumerable<MessageDto>?)> GetGroupMessagesAsync(Guid groupGuid, Guid? lastMessageId, int take)
    {
        var cacheKey = CacheKeys.GroupMessages(groupGuid);

        // Only get cached when fetching latest
        if (lastMessageId == null)
        {
            var cached = await _cache.ListRangeAsync(cacheKey, 0, -1);
            if (cached != null && cached.Length > 0)
            {
                var cachedMessages = cached.Select(x => JsonSerializer.Deserialize<MessageDto>(x)!)
                    .Take(take)
                    .ToList();
                return (true, "SuccessGotGroupMessages", cachedMessages);
            }
        }

        // No cached = DB fetch
        var query = _dbContext.Messages
            .AsNoTracking()
            .Where(x => x.GroupId == groupGuid);

        if (lastMessageId != null)
        {
            var lastCreatedAt = await _dbContext.Messages
                .Where(m => m.Id == lastMessageId)
                .Select(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastCreatedAt != default)
                query = query.Where(m => m.CreatedAt < lastCreatedAt);
        }

        var messages = await query
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
                Attachment = x.Attachment != null ? new MessageAttachmentDto
                {
                    FileName = x.Attachment.FileName,
                    FileType = x.Attachment.FileType,
                    FileSize = x.Attachment.FileSize
                } : null,
            })
            .ToListAsync();

        // Cache latest only when fetching latest
        if (lastMessageId == null)
        {
            if (messages.Count > 0)
            {
                var redisValue = messages.Select(x => JsonSerializer.Serialize(x)).ToArray();
                await _cache.ListRightPushAsync(cacheKey, redisValue);
                await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(5));
            }
        }

        return (true, "SuccessGotGroupMessages", messages);
    }

    public async Task<(bool, string, IEnumerable<GroupRoleDto>?)> GetGroupRolesAsync(Guid groupId)
    {
        var groupExists = await _dbContext.Groups
             .AsNoTracking()
             .AnyAsync(g => g.Id == groupId);

        if (!groupExists)
            return (false, "ErrorGroupNotFound", null);

        var roles = await _dbContext.GroupRoles
            .AsNoTracking()
            .Where(r => r.GroupId == groupId)
            .Select(r => new GroupRoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Color = r.Color,
                Permissions = r.Permissions
                    .Select(p => p.Permission.Name)
                    .ToList()
            })
            .ToListAsync();

        return (true, "SuccessGotGroupRoles", roles);
    }

    public async Task<(bool, string, MessageDto?)> SendMessageAsync(Guid groupId, SendMessageRequest request)
    {
        var userId = _httpContextAccessor.GetUserId();

        var group = await _dbContext.Groups
            .SingleOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var message = new Message
        {
            Content = request.Content,
            UserId = userId,
            GroupId = groupId,
        };

        if (request.Attachment != null)
        {
            var (success, uploadMessage, imageName) = await _uploadService.UploadFileAsync(request.Attachment);
            if (!success || imageName == null)
                return (false, uploadMessage, null);

            message.Attachment = new MessageAttachment
            {
                FileName = imageName,
                FileType = request.Attachment.ContentType,
                FileSize = request.Attachment.Length
            };

        }

        group.LastActivity = DateTime.UtcNow;

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        var messageDto = new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UserId = message.UserId,
            Attachment = message.Attachment != null ? new MessageAttachmentDto
            {
                FileName = message.Attachment.FileName,
                FileType = message.Attachment.FileType,
                FileSize = message.Attachment.FileSize
            } : null,
        };

        var cacheKey = CacheKeys.GroupMessages(groupId);
        var messageJson = JsonSerializer.Serialize(messageDto);
        await _cache.ListLeftPushAsync(cacheKey, messageJson);
        await _cache.ListTrimAsync(cacheKey, 0, 39);
        await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(5));

        return (true, "SuccessSentMessage",  messageDto);
    }

    public async Task<(bool, string, AddRoleToUserDto?)> AddRoleToUser(Guid groupId, string userId, Guid roleId)
    {
        var roleExists = await _dbContext.GroupRoles
            .AsNoTracking()
            .AnyAsync(gr => gr.Id == roleId && gr.GroupId == groupId );

        if (!roleExists)
            return (false, "ErrorRoleNotFound", null);

        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
            return (false, "ErrorUserNotFound", null);

        var alreadyHasRole = await _dbContext.UserGroupRoles
            .AsNoTracking()
            .AnyAsync(ugr => ugr.UserId == userId && ugr.GroupRoleId == roleId );

        if (alreadyHasRole)
            return (false, "ErrroUserAlreadyHasRole", null);

        var userGroupRole = new UserGroupRole
        {
            UserId = userId,
            GroupRoleId = roleId
        };

        _dbContext.UserGroupRoles.Add(userGroupRole);
        await _dbContext.SaveChangesAsync();

        await _cache.KeyDeleteAsync(CacheKeys.GroupMembers(groupId));

        var dto = new AddRoleToUserDto
        {
            GroupId = groupId,
            UserId = userId,
            RoleId = roleId
        };

        return (true, "SuccessAddedRoleToUser", dto);
    }

    public async Task<(bool, string, RemoveRoleFromUserDto?)> RemoveRoleFromUser(Guid groupId, string userId, Guid roleId)
    {
        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
            return (false, "ErrorUserNotFound", null);

        var userGroupRole = await _dbContext.UserGroupRoles
            .SingleOrDefaultAsync(ugr => ugr.UserId == userId && ugr.GroupRoleId == roleId);

        if (userGroupRole == null)
            return (false, "ErrorUserDoesNotHaveRole", null);

        _dbContext.UserGroupRoles.Remove(userGroupRole);
        await _dbContext.SaveChangesAsync();

        await _cache.KeyDeleteAsync(CacheKeys.GroupMembers(groupId));

        var dto = new RemoveRoleFromUserDto
        {
            GroupId = groupId,
            UserId = userId,
            RoleId = roleId
        };

        return (true, "SuccessRemovedRoleFromUser", dto);
    }

    public async Task<(bool, string, GroupRoleDto?)> CreateRoleAsync(Guid groupId, CreateGroupRoleRequest request)
    {
        var groupExists = await _dbContext.Groups
            .AsNoTracking()
            .AnyAsync(g => g.Id == groupId);

        if (!groupExists)
            return (false, "ErrorGroupNotFound", null);

        var permissionEntities = await _dbContext.Permissions
            .Where(p => request.Permissions.Contains(p.Name))
            .ToListAsync();

        var groupRole = new GroupRole
        {
            GroupId = groupId,
            Name = request.Name,
            Color = request.Color,
            Permissions = permissionEntities
                .Select(p => new GroupRolePermission
                {
                    PermissionId = p.Id,
                }).ToList()
        };

        _dbContext.GroupRoles.Add(groupRole);
        await _dbContext.SaveChangesAsync();

        var groupRoleDto = new GroupRoleDto
        {
            Id = groupRole.Id,
            Name = groupRole.Name,
            Color = groupRole.Color,
            Permissions = permissionEntities.Select(p => p.Name).ToList()
        };

        return (true, "SuccessCreatedGroupRole", groupRoleDto);
    }

    public async Task<(bool, string, GroupRoleDto?)> EditRoleAsync(Guid roleId, EditGroupRoleRequest request)
    {
        var groupRole = await _dbContext.GroupRoles
            .Include(gr => gr.Permissions)
            .SingleOrDefaultAsync(gr => gr.Id == roleId);

        if (groupRole == null)
            return (false, "ErrorRoleNotFound", null);

        groupRole.Name = request.Name;
        groupRole.Color = request.Color;

        _dbContext.GroupRolePermissions.RemoveRange(groupRole.Permissions);

        var permissionEntities = await _dbContext.Permissions
            .Where(p => request.Permissions.Contains(p.Name))
            .ToListAsync();

        groupRole.Permissions = permissionEntities
            .Select(p => new GroupRolePermission { GroupRoleId = groupRole.Id, PermissionId = p.Id })
            .ToList();

        await _dbContext.SaveChangesAsync();

        var groupRoleDto = new GroupRoleDto
        {
            Id = groupRole.Id,
            Name = groupRole.Name,
            Color = groupRole.Color,
            Permissions = permissionEntities.Select(p => p.Name).ToList()
        };

        return (true, "SuccessEditedGroupRole", groupRoleDto);
    }

    public async Task<(bool, string)> MarkAsReadAsync(Guid groupId, MarkAsReadRequest request)
    {
        var userId = _httpContextAccessor.GetUserId();

        var userGroup = await _dbContext.UserGroups
            .SingleOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null)
            return (false, "ErrorUserNotInGroup");

        userGroup.LastReadMessageId = request.LastMessageId;

        await _dbContext.SaveChangesAsync();

        return (true, "SuccessMarkedAsRead");
    }

    public async Task<(bool, string, GroupDto?)> EditGroupAsync(Guid groupId, EditGroupRequest request)
    {
        var group = _dbContext.Groups.SingleOrDefault(g => g.Id == groupId);
        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        group.Name = request.Name;

        await _dbContext.SaveChangesAsync();

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            OwnerId = group.OwnerId
        };

        return (true, "SuccessEditedGroup", groupDto);
    }

    public async Task<(bool, string)> DeleteGroupAsync(Guid groupId)
    {
        var group = await _dbContext.Groups.SingleOrDefaultAsync(g => g.Id == groupId);
        if (group == null) return (false, "ErrorGroupNotFound");

        _dbContext.Groups.Remove(group);
        await _dbContext.SaveChangesAsync();

        return (true, "SuccessDeletedGroup");
    }

    public async Task<(bool, string, JoinInviteDto?)> JoinWithInviteAsync(string inviteString)
    {
        var userId = _httpContextAccessor.GetUserId();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser", null);

        var checks = await _dbContext.Invites
            .AsNoTracking()
            .Where(x => x.InviteString == inviteString)
            .Select(i => new
            {
                GroupId = i.Group.Id,
                IsBanned = i.Group.Bans.Any(b => b.UserId == user.Id),
                isMember = i.Group.UserGroups.Any(ug => ug.UserId == user.Id),
                i.ValidThrough
            }).SingleOrDefaultAsync();

        if (checks == null)
            return (false, "ErrorInviteNotFound", null);

        if (checks.ValidThrough < DateOnly.FromDateTime(DateTime.UtcNow))
            return (false, "ErrorInviteExpired", null);

        if (checks.isMember)
            return (false, "ErrorAlreadyInGroup", null);

        if (checks.IsBanned)
            return (false, "ErrorBannedFromGroup", null);

        var inviteDto = await _dbContext.Invites
            .Where(x => x.InviteString == inviteString)
            .Select(i => new JoinInviteDto
            {
                Group = new GroupDto
                {
                    Id = i.Group.Id,
                    OwnerId = i.Group.OwnerId,
                    Name = i.Group.Name,
                    LastActivity = i.Group.LastActivity,
                    LatestMessage = i.Group.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.Content)
                        .FirstOrDefault(),
                },
                Member = new GroupMemberDto
                {
                    User = new UserDto
                    {
                        Id = user.Id,
                        Avatar = user.AvatarUri,
                        CreatedAt = user.CreatedAt,
                        Email = user.Email,
                        Username = user.UserName
                    }
                }
            })
            .SingleOrDefaultAsync();

        _dbContext.UserGroups.Add(new UserGroup
        {
            UserId = user.Id,
            GroupId = inviteDto.Group.Id,
            JoinedAt = DateTime.UtcNow,
        });

        await _dbContext.SaveChangesAsync();

        var cacheKey = CacheKeys.GroupMembers(inviteDto.Group.Id);

        await _cache.KeyDeleteAsync(cacheKey);

        return (true, "SuccessJoinedGroup", inviteDto);
    }

    public async Task<(bool, string, IEnumerable<BanDto>?)> GetGroupBansAsync(Guid groupId)
    {
        var bansDto = await _dbContext.Bans
            .Where(x => x.GroupId == groupId)
            .Select(x => new BanDto
            {
                Id = x.Id,
                BannedByUser = new UserDto
                {
                    Id = x.BannedByUser.Id,
                    Avatar = x.BannedByUser.AvatarUri,
                    Username = x.BannedByUser.UserName,
                    Email = x.BannedByUser.Email,
                },
                UserDto = new UserDto
                {
                    Id = x.UserId,
                    Avatar = x.User.AvatarUri,
                    Username = x.User.UserName,
                    Email = x.User.Email
                },
                CreatedAt = x.CreatedAt,
                Reason = x.Reason,

            })
            .ToListAsync();

        if (!bansDto.Any())
        {
            var groupExists = await _dbContext.Groups.AnyAsync(x => x.Id == groupId);
            if (!groupExists)
                return (false, "ErrorGroupNotFound", null);
        }

        return (true, "SuccessGotGroupBans", bansDto);
    }

    public async Task<(bool, string)> DeleteGroupBanAsync(Guid groupId, Guid banId)
    {
        var ban = await _dbContext.Bans
            .Where(x => x.GroupId == groupId && x.Id == banId)
            .SingleOrDefaultAsync();

        if (ban == null)
            return (false, "ErrorGroupOrBanNotFound");

        _dbContext.Bans.Remove(ban);
        await _dbContext.SaveChangesAsync();

        return (true, "SuccessDeletedBan");
    }
}