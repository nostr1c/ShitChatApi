namespace ShitChat.Application.DTOs
{
    public class AddRoleToUserDto
    {
        public Guid GroupId { get; set; }
        public string UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
