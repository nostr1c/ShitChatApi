using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Groups.Requests;
using ShitChat.Application.Users.DTOs;

namespace ShitChat.Application.Groups.Services;

public interface IGroupService
{
    Task<(bool, string, GroupDto?)> CreateGroupAsync(CreateGroupRequest request);
    Task<(bool, string, GroupDto?)> GetGroupByGuidAsync(Guid groupId);
    Task<(bool, string, UserDto?)> AddUserToGroupAsync(Guid groupId, string userId);
    Task<(bool, string)> KickUserFromGroupAsync(Guid groupId, string userId);
    Task<(bool, string)> BanUserFromGroupAsync(Guid groupId, string userId, BanUserRequest request);
    Task<(bool, string, IEnumerable<GroupMemberDto>?)> GetGroupMembersAsync(Guid groupId);
    Task<(bool, string, IEnumerable<MessageDto>?)> GetGroupMessagesAsync(Guid groupGuid, Guid? lastMessageId, int take);
    Task<(bool, string, MessageDto?)> SendMessageAsync(Guid groupId, SendMessageRequest request);
    Task<(bool, string, IEnumerable<GroupRoleDto>?)> GetGroupRolesAsync(Guid groupId);
    Task<(bool, string, AddRoleToUserDto?)> AddRoleToUser(Guid groupId, string userId, Guid roleId);
    Task<(bool, string, RemoveRoleFromUserDto?)> RemoveRoleFromUser(Guid groupId, string userId, Guid roleId);
    Task<(bool, string, GroupRoleDto?)> CreateRoleAsync(Guid groupId, CreateGroupRoleRequest request);
    Task<(bool, string, GroupRoleDto?)> EditRoleAsync(Guid roleId, EditGroupRoleRequest request);
    Task<(bool, string)> MarkAsReadAsync(Guid groupId, MarkAsReadRequest request);
}
