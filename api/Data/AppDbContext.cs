using api.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

    public DbSet<Connection> Connections { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Message> Messages { get; set; }

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

        builder.Entity<User>()
            .HasMany(u => u.Groups)
            .WithMany(g => g.Users)
            .UsingEntity(j => j.ToTable("UserGroups"));

        builder.Entity<Message>()
            .HasOne(m => m.Group)
            .WithMany(g => g.Messages)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        string user1Email = "alice.smith@example.com";
        string user1UserName = "alice123";

        string user2Email = "bob.jones@example.com";
        string user2UserName = "BobLord1337";

        string user3Email = "carla.davis@example.com";
        string user3UserName = "Carlis";

        string user4Email = "david.lee@example.com";
        string user4UserName = "DavidLee";

        string user5Email = "emily.white@example.com";
        string user5UserName = "EmilyCool";

        string user6Email = "frank.hall@example.com";
        string user6UserName = "FrankTheMan";

        var user1 = new User
        {
            Id = "bb29d713-9414-43fa-9c8e-65fa6ee39243",
            AvatarUri = "a2138670-ffb4-466c-b40c-44dde76566ed.jpg",
            RefreshToken = "exampletoken1",
            RefreshTokenExpiryTime = new DateTime(2024, 12, 23),
            UserName = user1UserName,
            NormalizedUserName = user1UserName.ToUpper(),
            Email = user1Email,
            NormalizedEmail = user1Email.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", // 12345
            SecurityStamp = "",
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        var user2 = new User
        {
            Id = "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
            AvatarUri = "a2138670-ffb4-466c-b40c-44dde76566ed.jpg",
            RefreshToken = "exampletoken1",
            RefreshTokenExpiryTime = new DateTime(2024, 12, 23),
            UserName = user2UserName,
            NormalizedUserName = user2UserName.ToUpper(),
            Email = user2Email,
            NormalizedEmail = user2Email.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", // 12345
            SecurityStamp = "",
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        var user3 = new User
        {
            Id = "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
            AvatarUri = "a2138670-ffb4-466c-b40c-44dde76566ed.jpg",
            RefreshToken = "exampletoken1",
            RefreshTokenExpiryTime = new DateTime(2024, 12, 23),
            UserName = user3UserName,
            NormalizedUserName = user3UserName.ToUpper(),
            Email = user3Email,
            NormalizedEmail = user3Email.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", // 12345
            SecurityStamp = "",
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        var user4 = new User
        {
            Id = "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
            AvatarUri = "a2138670-ffb4-466c-b40c-44dde76566ed.jpg",
            RefreshToken = "exampletoken1",
            RefreshTokenExpiryTime = new DateTime(2024, 12, 23),
            UserName = user4UserName,
            NormalizedUserName = user4UserName.ToUpper(),
            Email = user4Email,
            NormalizedEmail = user4Email.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", // 12345
            SecurityStamp = "",
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        var user5 = new User
        {
            Id = "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
            AvatarUri = "a2138670-ffb4-466c-b40c-44dde76566ed.jpg",
            RefreshToken = "exampletoken1",
            RefreshTokenExpiryTime = new DateTime(2024, 12, 23),
            UserName = user5UserName,
            NormalizedUserName = user5UserName.ToUpper(),
            Email = user5Email,
            NormalizedEmail = user5Email.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", // 12345
            SecurityStamp = "",
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        var user6 = new User
        {
            Id = "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
            AvatarUri = "a2138670-ffb4-466c-b40c-44dde76566ed.jpg",
            RefreshToken = "exampletoken1",
            RefreshTokenExpiryTime = new DateTime(2024, 12, 23),
            UserName = user6UserName,
            NormalizedUserName = user6UserName.ToUpper(),
            Email = user6Email,
            NormalizedEmail = user6Email.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", // 12345
            SecurityStamp = "",
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };

        // Insert users
        builder.Entity<User>().HasData(
            user1, user2, user3, user4, user5, user6
        );


        builder.Entity<Connection>().HasData(
            new Connection
            {
                id = Guid.Parse("dcdf519b-70a5-448a-8599-515d9da297d9"),
                UserId = user1.Id,
                FriendId = user2.Id,
                Accepted = true,
                CreatedAt = new DateTime(2024, 12, 23),
            },
            new Connection
            {
                id = Guid.Parse("60f01455-f4f7-4986-9489-75ffd3d699e0"),
                UserId = user1.Id,
                FriendId = user3.Id,
                Accepted = true,
                CreatedAt = new DateTime(2024, 12, 23),
            },
            new Connection
            {
                id = Guid.Parse("a875b4fb-f2b1-4a68-8684-4ab4915c002d"),
                UserId = user1.Id,
                FriendId = user4.Id,
                Accepted = true,
                CreatedAt = new DateTime(2024, 12, 23),
            },
            new Connection
            {
                id = Guid.Parse("bbb967e5-a6b5-41c0-9d69-6a72cc05a12f"),
                UserId = user1.Id,
                FriendId = user5.Id,
                Accepted = true,
                CreatedAt = new DateTime(2024, 12, 23),
            },
            new Connection
            {
                id = Guid.Parse("cfa42098-b22a-4272-b9d9-52f4b57de616"),
                UserId = user1.Id,
                FriendId = user6.Id,
                Accepted = true,
                CreatedAt = new DateTime(2024, 12, 23),
            }
        );
    }
}
