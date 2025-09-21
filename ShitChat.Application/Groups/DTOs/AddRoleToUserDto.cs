namespace ShitChat.Application.Groups.DTOs
{
    public class AddRoleToUserDto
    {
        public Guid GroupId { get; set; }
        public required string UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
