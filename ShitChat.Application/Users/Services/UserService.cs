using Azure;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShitChat.Application.Connections.DTOs;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Roles.DTOs;
using ShitChat.Application.Uploads.Services;
using ShitChat.Application.Users.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Enums;
using ShitChat.Shared.Extensions;
using SixLabors.ImageSharp;

namespace ShitChat.Application.Users.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IUploadService _uploadService;
    private readonly ElasticsearchClient _elastic;

    public UserService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager,
        IUploadService uploadService,
        ElasticsearchClient elastic
    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _uploadService = uploadService;
        _elastic = elastic;
    }

    public async Task<(bool, UserActionResult, UserDto?)> GetUserByGuidAsync(string userGuid)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userGuid);

        if (user == null)
            return (false, UserActionResult.ErrorUserNotFound, null);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Avatar = user.AvatarUri,
            CreatedAt = user.CreatedAt
        };

        return (true, UserActionResult.SuccessGotUser, userDto);
    }

    public async Task<(bool, UserActionResult?, UploadActionResult?, string?)> UpdateAvatarAsync(IFormFile avatar)
    {
        var userId = _httpContextAccessor.GetUserId();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, UserActionResult.ErrorUserNotFound, null, null);

        var (success, message, imageName) = await _uploadService.UploadFileAsync(avatar, 400, 400);
        if (!success || imageName == null)
            return (false, null, message, null);

        user.AvatarUri = imageName;
        await _userManager.UpdateAsync(user);


        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Avatar = user.AvatarUri
        };

        await _elastic.IndexAsync(userDto);

        return (true, UserActionResult.SuccessUpdatedAvatar, null, imageName);
    }

    public async Task<(bool, UserActionResult, ConnectionsDto?)> GetConnectionsAsync()
    {
        var userId = _httpContextAccessor.GetUserId();
        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId);

        if (!userExists)
            return (false, UserActionResult.ErrorUserNotFound, null);

        var allConnections = await _dbContext.Connections
            .AsNoTracking()
            .Where(x => x.UserId == userId || x.FriendId == userId)
            .Select(x => new
            {
                x.id,
                x.Accepted,
                x.CreatedAt,
                IsRequester = x.UserId == userId,
                Friend = new UserDto
                {
                    Id = x.UserId == userId ? x.FriendId : x.UserId,
                    Username = x.UserId == userId ? x.friend.UserName : x.user.UserName,
                    Email = x.UserId == userId ? x.friend.Email : x.user.Email,
                    Avatar = x.UserId == userId ? x.friend.AvatarUri : x.user.AvatarUri,
                    CreatedAt = x.UserId == userId ? x.friend.CreatedAt : x.user.CreatedAt
                }
            })
            .ToListAsync();

        var result = new ConnectionsDto
        {
            SentRequests = allConnections
                .Where(x => !x.Accepted && x.IsRequester)
                .Select(x => new ConnectionDto
                {
                    Id = x.id,
                    Accepted = x.Accepted,
                    CreatedAt = x.CreatedAt,
                    IsRequester = x.IsRequester,
                    User = x.Friend
                }),
            ReceivedRequests = allConnections
                .Where(x => !x.Accepted && !x.IsRequester)
                .Select(x => new ConnectionDto
                {
                    Id = x.id,
                    Accepted = x.Accepted,
                    CreatedAt = x.CreatedAt,
                    IsRequester = x.IsRequester,
                    User = x.Friend
                }),
            Accepted = allConnections
                .Where(x => x.Accepted)
                .Select(x => new ConnectionDto
                {
                    Id = x.id,
                    Accepted = x.Accepted,
                    CreatedAt = x.CreatedAt,
                    IsRequester = x.IsRequester,
                    User = x.Friend
                })
        };

        return (true, UserActionResult.SuccessGotUserConnections, result);
    }

    public async Task<(bool, UserActionResult, List<GroupDto>?)> GetUserGroupsAsync()
    {
        var userId = _httpContextAccessor.GetUserId();

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

        return (true, UserActionResult.SuccessGotUserGroups, groups);
    }

    public async Task<(bool, string, IEnumerable<UserWithRoles>)> GetUsersWithRolesAsync()
    {
        var usersWithRoles = await _dbContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any())
            .Select(u => new UserWithRoles
            {
                User = new UserDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    Avatar = u.AvatarUri,
                    CreatedAt = u.CreatedAt
                },
                Roles = u.UserRoles
                    .Select(ur => new RoleDto
                    {
                        Id = ur.RoleId,
                        Name = ur.Role.Name
                    }).ToList()
            })
            .AsNoTracking()
            .ToListAsync();


        return (true, "SuccessGotUsersWithRoles", usersWithRoles);
    }

    public async Task<(bool, string?)> IndexAllUsersAsync()
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Select(x => new UserDto
            {
                Id = x.Id,
                Username = x.UserName,
                Email = x.Email,
                CreatedAt = x.CreatedAt,
                Avatar = x.AvatarUri
            })
            .ToListAsync();

        if (users == null || users.Count == 0)
        {
            return (false, "No users found");
        }

        var deleteResponse = await _elastic.Indices.DeleteAsync("users");
        if (!deleteResponse.IsValidResponse)
        {
            return (false, deleteResponse.ElasticsearchServerError?.Error?.Reason ?? "Unknown Elasticsearch error");
        }

        var bulkResponse = await _elastic.BulkAsync(b => b
            .Index("users")
            .IndexMany(users, (descriptor, user) => descriptor.Id(user.Id))
        );

        if (!bulkResponse.IsValidResponse)
        {
            return (false, bulkResponse.ElasticsearchServerError?.Error?.Reason ?? "Unknown Elasticsearch error");
        }


        return (true, $"Successfully indexed {users.Count} users ");
    }

    public async Task<(bool, string?, IReadOnlyCollection<UserDto>?)> SearchUsersAsync(string query)
    {
        var result = await _elastic.SearchAsync<UserDto>(s => s
            .Query(q => q
                .MatchPhrasePrefix(m => m
                    .Field(f => f.Username)
                    .Query(query)
                )
            )
        );

        if (!result.IsValidResponse)
            return (false, result.ElasticsearchServerError?.Error?.Reason, null);

        return (true, "SuccessGotUsers", result.Documents);
    }
}