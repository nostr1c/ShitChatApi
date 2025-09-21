namespace ShitChat.Application.Groups.DTOs
{
    public class RemoveRoleFromUserDto
    {
        public Guid GroupId { get; set; }
        public required string UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
