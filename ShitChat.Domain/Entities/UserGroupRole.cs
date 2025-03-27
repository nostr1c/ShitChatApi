using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class UserGroupRole
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid GroupRoleId { get; set; }

    // Navigation
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    [ForeignKey(nameof(GroupRoleId))]
    public GroupRole GroupRole { get; set; }
}
