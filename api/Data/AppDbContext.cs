using api.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure the Connection entity
            builder.Entity<Connection>()
                .HasOne(c => c.user)
                .WithMany(u => u.Connections)  // One user can have many connections
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            builder.Entity<Connection>()
                .HasOne(c => c.friend)
                .WithMany()  // A friend may not have any connections back (optional)
                .HasForeignKey(c => c.FriendId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
        }
    }
}
