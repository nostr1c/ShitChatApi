namespace api.Data.Models
{
    public class UserGroup
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
