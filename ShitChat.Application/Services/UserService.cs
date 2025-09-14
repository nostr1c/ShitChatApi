using ShitChat.Infrastructure.Data;
using ShitChat.Domain.Entities;
using ShitChat.Shared.Extensions;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;

namespace ShitChat.Application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly string _imageStoragePath = "/Uploads";

    public UserService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager
    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;

        if (!Directory.Exists(_imageStoragePath))
        {
            Directory.CreateDirectory(_imageStoragePath);
        }
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
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        if (avatar == null || avatar.Length == 0)
            return (false, "ErrorInvalidFile", null);

        string[] allowedExtensions = [".jpg", ".jpeg", ".png"];

        var fileExtension = Path.GetExtension(avatar.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
            return (false, "ErrorNotValidFileFormat", null);

        string imageId = Guid.NewGuid().ToString();
        string imageName = $"{imageId}{fileExtension}";
        string ImagePath = Path.Combine(_imageStoragePath, imageName);

        using var image = await Image.LoadAsync(avatar.OpenReadStream());

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(400, 400),
            Mode = ResizeMode.Crop
        }));

        using (var fileStream = new FileStream(ImagePath, FileMode.Create))
        {
            await image.SaveAsWebpAsync(fileStream);
        }

        user.AvatarUri = imageName;
        await _userManager.UpdateAsync(user);

        return (true, "SuccessUpdatedAvatar", imageName);
    }

    public async Task<(bool, string, List<ConnectionDto>)> GetConnectionsAsync()
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
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

    public async Task<(bool, string, List<GroupDto>)> GetUserGroupsAsync()
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var groups = await _dbContext.Groups
            .AsNoTracking()
            .Where(g => g.UserGroups.Any(ug => ug.UserId == userId))
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                LatestMessage = g.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
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