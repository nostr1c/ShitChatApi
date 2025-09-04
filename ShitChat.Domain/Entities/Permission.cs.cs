namespace ShitChat.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }

    // Navigation props
    public ICollection<GroupRolePermission> GroupRoles { get; set; }
}
