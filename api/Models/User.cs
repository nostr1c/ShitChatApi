namespace api.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Created_At { get; set; }
        public int IsDeleted { get; set; }
    }
}
