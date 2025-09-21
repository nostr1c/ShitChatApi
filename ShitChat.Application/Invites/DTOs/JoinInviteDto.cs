using ShitChat.Application.Groups.DTOs;

namespace ShitChat.Application.Invites.DTOs;

public class JoinInviteDto
{
    public required GroupDto Group { get; set; }
    public required GroupMemberDto Member { get; set; }
}
