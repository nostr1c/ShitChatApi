namespace ShitChat.Domain.Entities;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public string Action { get; set; }
}
