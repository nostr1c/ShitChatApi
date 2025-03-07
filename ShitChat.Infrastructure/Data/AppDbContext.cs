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
            .UsingEntity<Dictionary<string, object>>(
                "UserGroups",
                j => j.HasOne<Group>().WithMany().HasForeignKey("GroupsId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("UsersId"),
                j =>
                {
                    j.HasKey("GroupsId", "UsersId");
                }
            );

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

        var group1 = Guid.Parse("be081304-63c6-4cae-bf25-b7e33cc6e495");
        var group2 = Guid.Parse("25d5ec2b-ebe4-4462-ada9-17c246fb5273");
        var group3 = Guid.Parse("f50053c8-7fd8-498b-8c0b-30277bc378b0");
        var group4 = Guid.Parse("707e730d-0d72-4109-b9d9-5dc47b637268");
        var group5 = Guid.Parse("c7fcbe94-0f5f-47e6-9b71-5cc04ce32538");

        builder.Entity<Group>().HasData(
            new Group
            {
                Id = group1,
                Name = "Group 1",
                OwnerId = user1.Id
            },
            new Group
            {
                Id = group2,
                Name = "Group 2",
                OwnerId = user1.Id
            },
            new Group
            {
                Id = group3,
                Name = "Group 3",
                OwnerId = user3.Id
            },
            new Group
            {
                Id = group4,
                Name = "Group 4",
                OwnerId = user4.Id
            },
            new Group
            {
                Id = group5,
                Name = "Group 5",
                OwnerId = user5.Id
            }
        );

        builder.Entity("UserGroups").HasData(
            new
            {
                GroupsId = group1,
                UsersId = user1.Id
            },
            new
            {
                GroupsId = group1,
                UsersId = user2.Id
            },
            new
            {
                GroupsId = group2,
                UsersId = user1.Id
            },
            new
            {
                GroupsId = group3,
                UsersId = user1.Id
            },
            new
            {
                GroupsId = group4,
                UsersId = user1.Id
            },
            new
            {
                GroupsId = group5,
                UsersId = user1.Id
            }
        );

        builder.Entity<Message>().HasData(
            new Message
            {
                Id = Guid.Parse("7acbca3e-d27a-403c-af5b-37b31e4bf53a"),
                Content = "Hej",
                CreatedAt = new DateTime(2025, 01, 15),
                GroupId = group1,
                UserId = user1.Id
            },
            new Message
            {
                Id = Guid.Parse("1baf7663-c7da-4954-bd1c-2865de582301"),
                Content = "Nämen tjena",
                CreatedAt = new DateTime(2025, 01, 15),
                GroupId = group1,
                UserId = user2.Id
            }
        );
    }
}
