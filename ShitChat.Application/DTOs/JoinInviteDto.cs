namespace ShitChat.Application.DTOs;

public class JoinInviteDto
{
    public Guid Group { get; set; }
    public GroupMemberDto Member { get; set; }
}
