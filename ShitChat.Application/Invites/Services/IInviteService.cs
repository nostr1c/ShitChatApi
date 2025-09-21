using ShitChat.Application.Invites.DTOs;
using ShitChat.Application.Invites.Requests;

namespace ShitChat.Application.Invites.Services;

public interface IInviteService
{
    Task<(bool, string, InviteDto?)> CreateInviteAsync(Guid groupGuid, CreateInviteRequest request);
    Task<(bool, string, JoinInviteDto?)> JoinWithInviteAsync(string inviteString);
    Task<(bool, string, IEnumerable<InviteDto>?)> GetGroupInvites(Guid groupGuid);
}
