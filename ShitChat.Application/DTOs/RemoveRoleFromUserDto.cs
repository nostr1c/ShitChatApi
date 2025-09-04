namespace ShitChat.Application.DTOs
{
    public class RemoveRoleFromUserDto
    {
        public Guid GroupId { get; set; }
        public string UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
