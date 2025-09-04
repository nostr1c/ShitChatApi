using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class GroupRolePermission
{
    public Guid GroupRoleId { get; set; }
    public Guid PermissionId { get; set; }

    // Navigation props
    [ForeignKey(nameof(GroupRoleId))]
    public GroupRole GroupRole { get; set; }
    [ForeignKey(nameof(PermissionId))]
    public Permission Permission { get; set; }
}
