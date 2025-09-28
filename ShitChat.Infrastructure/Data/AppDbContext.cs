using ShitChat.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShitChat.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

    public DbSet<Connection> Connections { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<GroupRole> GroupRoles { get; set; }
    public DbSet<UserGroupRole> UserGroupRoles { get; set; }
    public DbSet<Invite> Invites { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<GroupRolePermission> GroupRolePermissions { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<Ban> Bans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<Connection>()
            .HasOne(c => c.user)
            .WithMany(u => u.Connections)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Connection>()
            .HasOne(c => c.friend)
            .WithMany()
            .HasForeignKey(c => c.FriendId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Group>()
            .HasOne(g => g.Owner)
            .WithMany(u => u.OwnedGroups)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserGroup>()
             .HasKey(ug => new { ug.UserId, ug.GroupId });

        builder.Entity<UserGroup>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserGroup>()
            .HasOne(ug => ug.Group)
            .WithMany(g => g.UserGroups)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .HasOne(m => m.Group)
            .WithMany(g => g.Messages)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Message>()
            .HasOne(m => m.Attachment)
            .WithOne(ma => ma.Message)
            .HasForeignKey<MessageAttachment>(ma => ma.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MessageAttachment>()
            .HasIndex(ma => ma.FileName)
            .IsUnique();

        builder.Entity<GroupRole>()
            .HasOne(gr => gr.Group)
            .WithMany(g => g.Roles)
            .HasForeignKey(gr => gr.GroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserGroupRole>()
            .HasOne(ugr => ugr.User)
            .WithMany(u => u.GroupRoles)
            .HasForeignKey(gr => gr.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserGroupRole>()
            .HasOne(ugr => ugr.GroupRole)
            .WithMany()
            .HasForeignKey(gr => gr.GroupRoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Invite>()
            .HasOne(i => i.Creator)
            .WithMany()
            .HasForeignKey(i => i.UserId);

        builder.Entity<Invite>()
            .HasOne(i => i.Group)
            .WithMany(g => g.Invites)
            .HasForeignKey(i => i.GroupId);

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);

        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.TokenHash)
            .IsUnique();

        builder.Entity<GroupRolePermission>()
            .HasKey(gp => new { gp.GroupRoleId, gp.PermissionId });

        builder.Entity<GroupRolePermission>()
            .HasOne(gp => gp.GroupRole)
            .WithMany(gr => gr.Permissions)
            .HasForeignKey(gp => gp.GroupRoleId);

        builder.Entity<GroupRolePermission>()
            .HasOne(gp => gp.Permission)
            .WithMany(p => p.GroupRoles)
            .HasForeignKey(gp => gp.PermissionId);

        builder.Entity<Ban>()
            .HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ban>()
            .HasOne(b => b.Group)
            .WithMany(g => g.Bans)
            .HasForeignKey(b => b.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ban>()
            .HasOne(b => b.BannedByUser)
            .WithMany()
            .HasForeignKey(b => b.BannedByUserId)
            .OnDelete(DeleteBehavior.Restrict);


        // Seeding permanents
        builder.Entity<Permission>()
            .HasData(new Permission
            {
                Id = Guid.Parse("bd74b2af-ed25-48a5-8ab4-f78227a58d06"),
                Name = "kick_user"
            },
            new Permission
            {
                Id = Guid.Parse("e5bebdec-1a32-4c9d-a54e-39cc0d073ed6"),
                Name = "ban_user"
            },
            new Permission
            {
                Id = Guid.Parse("c8161caf-eb44-4c71-baf1-eea17481989c"),
                Name = "manage_user_roles"
            },
            new Permission
            {
                Id = Guid.Parse("61e895bb-021f-42ae-88af-c7444931630e"),
                Name = "manage_server_roles"
            },
            new Permission
            {
                Id = Guid.Parse("037f49f3-9f9f-4c45-b94b-1b8c0e595fb9"),
                Name = "manage_invites"
            },
            new Permission
            {
                Id = Guid.Parse("47d3b3f7-2e55-4867-9bca-4e1f971ae5ae"),
                Name = "manage_server"
            },
            new Permission
            {
                Id = Guid.Parse("5a6ba92e-1013-4c73-9589-0dba08bfa2bf"),
                Name = "delete_messages"
            }
            );
    }
}
