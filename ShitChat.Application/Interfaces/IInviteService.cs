using ShitChat.Application.DTOs;
using ShitChat.Application.Requests;

namespace ShitChat.Application.Interfaces;

public interface IInviteService
{
    Task<(bool, string, InviteDto?)> CreateInviteAsync(Guid groupGuid, CreateInviteRequest request);
    Task<(bool, string, JoinInviteDto?)> JoinWithInviteAsync(string inviteString);
}
