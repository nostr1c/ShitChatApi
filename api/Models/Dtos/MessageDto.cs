namespace api.Models.Dtos
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public MessageUserDto User { get; set; }
    }
}
