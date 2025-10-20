using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Groups.Requests;
using ShitChat.Application.Invites.DTOs;
using ShitChat.Application.Users.DTOs;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Groups.Services;

public interface IGroupService
{
    Task<(bool, GroupActionResult, GroupDto?)> CreateGroupAsync(CreateGroupRequest request);
    Task<(bool, GroupActionResult, UserDto?)> AddUserToGroupAsync(Guid groupId, string userId);
    Task<(bool, GroupActionResult)> KickUserFromGroupAsync(Guid groupId, string userId);
    Task<(bool, GroupActionResult, BanDto?)> BanUserFromGroupAsync(Guid groupId, string userId, BanUserRequest request);
    Task<(bool, GroupActionResult, GroupDto?)> GetGroupByGuidAsync(Guid groupId);
    Task<(bool, GroupActionResult, IEnumerable<GroupMemberDto>?)> GetGroupMembersAsync(Guid groupId);
    Task<(bool, GroupActionResult, IEnumerable<MessageDto>?)> GetGroupMessagesAsync(Guid groupGuid, Guid? lastMessageId, int take);
    Task<(bool, GroupActionResult?, UploadActionResult?, MessageDto?)> SendMessageAsync(Guid groupId, SendMessageRequest request);
    Task<(bool, GroupActionResult, IEnumerable<GroupRoleDto>?)> GetGroupRolesAsync(Guid groupId);
    Task<(bool, GroupActionResult, AddRoleToUserDto?)> AddRoleToUser(Guid groupId, string userId, Guid roleId);
    Task<(bool, GroupActionResult, RemoveRoleFromUserDto?)> RemoveRoleFromUser(Guid groupId, string userId, Guid roleId);
    Task<(bool, GroupActionResult, GroupRoleDto?)> CreateRoleAsync(Guid groupId, CreateGroupRoleRequest request);
    Task<(bool, GroupActionResult, GroupRoleDto?)> EditRoleAsync(Guid roleId, EditGroupRoleRequest request);
    Task<(bool, GroupActionResult)> MarkAsReadAsync(Guid groupId, MarkAsReadRequest request);
    Task<(bool, GroupActionResult, GroupDto?)> EditGroupAsync(Guid groupId, EditGroupRequest request);
    Task<(bool, GroupActionResult)> DeleteGroupAsync(Guid groupId);
    Task<(bool, GroupActionResult, JoinInviteDto?)> JoinWithInviteAsync(string inviteString);
    Task<(bool, GroupActionResult, IEnumerable<BanDto>?)> GetGroupBansAsync(Guid groupId);
    Task<(bool, GroupActionResult)> DeleteGroupBanAsync(Guid groupId, Guid banId);
}
