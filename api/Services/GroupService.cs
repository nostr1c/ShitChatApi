using api.Data;
using api.Data.Models;
using api.Extensions;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;

namespace api.Services
{
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupService
        (
            AppDbContext dbContext,
            IHttpContextAccessor httpContextAccessor
        ) { 
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
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
    }
}
