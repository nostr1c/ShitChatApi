namespace ShitChat.Application.DTOs;

public class GroupMemberDto
{
    public UserDto User { get; set; }
    public IEnumerable<GroupRoleDto> Roles { get; set; }
}
