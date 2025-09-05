namespace ShitChat.Application.DTOs;

public class GroupRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string?> Permissions { get; set; }
}