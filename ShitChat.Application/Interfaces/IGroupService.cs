using Application.DTOs;
using Application.Requests;

namespace Application.Interfaces;

public interface IGroupService
{
    Task<GroupDto> CreateGroupAsync(CreateGroupRequest request);
    Task<GroupDto> GetGroupByGuidAsync(Guid groupId);
    Task<List<GroupDto>> GetUserGroupsAsync();
    Task<(bool, string, UserDto?)> AddUserToGroupAsync(Guid groupId, string userId);
    Task<(bool, string, IEnumerable<UserDto>?)> GetGroupMembersAsync(Guid groupId);
    Task<(bool, string, IEnumerable<MessageDto>?)> GetGroupMessagesAsync(Guid groupId);
    Task<(bool, string, MessageDto?)> SendMessageAsync(Guid groupId, SendMessageRequest request);
}
