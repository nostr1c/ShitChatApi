using ShitChat.Application.Invites.DTOs;
using ShitChat.Application.Invites.Requests;

namespace ShitChat.Application.Invites.Services;

public interface IInviteService
{
    Task<(bool, string, InviteDto?)> CreateInviteAsync(Guid groupGuid, CreateInviteRequest request);
    Task<(bool, string, IEnumerable<InviteDto>?)> GetGroupInvites(Guid groupGuid);
    Task<(bool, string)> DeleteInviteAsync(Guid inviteId);
}
