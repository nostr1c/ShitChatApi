namespace ShitChat.Application.Groups.DTOs;

public class GroupRoleDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public List<string> Permissions { get; set; }= new List<string>();
}