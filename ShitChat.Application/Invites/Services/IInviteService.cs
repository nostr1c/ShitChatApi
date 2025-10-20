using ShitChat.Application.Invites.DTOs;
using ShitChat.Application.Invites.Requests;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Invites.Services;

public interface IInviteService
{
    Task<(bool, InviteActionResult, InviteDto?)> CreateInviteAsync(Guid groupGuid, CreateInviteRequest request);
    Task<(bool, InviteActionResult, IEnumerable<InviteDto>?)> GetGroupInvites(Guid groupGuid);
    Task<(bool, InviteActionResult)> DeleteInviteAsync(Guid inviteId);
}
