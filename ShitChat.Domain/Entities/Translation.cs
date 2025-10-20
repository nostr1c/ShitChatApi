namespace ShitChat.Domain.Entities;

public class Translation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
}
