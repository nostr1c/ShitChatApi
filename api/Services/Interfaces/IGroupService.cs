using api.Models.Dtos;
using api.Models.Requests;

namespace api.Services.Interfaces
{
    public interface IGroupService
    {
        Task<GroupDto> CreateGroupAsync(CreateGroupRequest request);
        Task<List<GroupDto>> GetUserGroupsAsync();
        Task<(bool, string, UserDto?)> AddUserToGroupAsync(Guid groupId, string userId);
    }
}
