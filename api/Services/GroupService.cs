using api.Data;
using api.Data.Models;
using api.Extensions;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GroupService> _logger;

        public GroupService
        (
            AppDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GroupService> logger
        )
        { 
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<(bool, string, UserDto?)> AddUserToGroupAsync(Guid groupId, string userId)
        {
            var group = await _dbContext.Groups
                .Include(x => x.Users)
                .SingleOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return (false, "ErrorGroupNotFound", null);

            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);
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

            var group = new Group
            {
                Name = request.Name,
                OwnerId = userId
            };

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

        public async Task<List<GroupDto>> GetUserGroupsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();

            var groups = await _dbContext.Groups
                .Where(x => x.Users.Any(x => x.Id == userId))
                .Select(x => new GroupDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    OwnerId = x.OwnerId,
                }).ToListAsync();

            return groups;
        }
    }
}
