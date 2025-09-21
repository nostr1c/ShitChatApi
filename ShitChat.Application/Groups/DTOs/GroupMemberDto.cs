using ShitChat.Application.Users.DTOs;

namespace ShitChat.Application.Groups.DTOs;

public class GroupMemberDto
{
    public required UserDto User { get; set; }
    public IEnumerable<Guid> Roles { get; set; } = new List<Guid>();
}
