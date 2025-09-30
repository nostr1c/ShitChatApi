using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class GroupRole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Color { get; set; }
    public Guid GroupId { get; set; }

    // Navigation
    [ForeignKey(nameof(GroupId))]
    public Group Group { get; set; }

    public ICollection<GroupRolePermission> Permissions { get; set; }
    public ICollection<UserGroupRole> Users { get; set; }
}
