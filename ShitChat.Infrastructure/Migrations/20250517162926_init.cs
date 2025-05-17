using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShitChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AvatarUri = table.Column<string>(type: "text", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FriendId = table.Column<string>(type: "text", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.id);
                    table.ForeignKey(
                        name: "FK_Connections_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Connections_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GroupRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupRoles_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidThrough = table.Column<DateOnly>(type: "date", nullable: false),
                    InviteString = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invites_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    GroupsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => new { x.GroupsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserGroups_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroups_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    GroupRoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroupRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserGroupRoles_GroupRoles_GroupRoleId",
                        column: x => x.GroupRoleId,
                        principalTable: "GroupRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AvatarUri", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "a1f2d713-1234-43fa-9c8e-65fa6ee39244", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 5, 17), "bob.jones@example.com", true, false, null, "BOB.JONES@EXAMPLE.COM", "BOBLORD1337", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "", false, "BobLord1337" },
                    { "bb29d713-9414-43fa-9c8e-65fa6ee39243", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 5, 17), "alice.smith@example.com", true, false, null, "ALICE.SMITH@EXAMPLE.COM", "ALICE123", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "", false, "alice123" },
                    { "c1f3d713-5678-43fa-9c8e-65fa6ee39245", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 5, 17), "carla.davis@example.com", true, false, null, "CARLA.DAVIS@EXAMPLE.COM", "CARLIS", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "", false, "Carlis" },
                    { "d1f4d713-9101-43fa-9c8e-65fa6ee39246", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 5, 17), "david.lee@example.com", true, false, null, "DAVID.LEE@EXAMPLE.COM", "DAVIDLEE", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "", false, "DavidLee" },
                    { "e1f5d713-1122-43fa-9c8e-65fa6ee39247", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 5, 17), "emily.white@example.com", true, false, null, "EMILY.WHITE@EXAMPLE.COM", "EMILYCOOL", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "", false, "EmilyCool" },
                    { "f1f6d713-3344-43fa-9c8e-65fa6ee39248", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 5, 17), "frank.hall@example.com", true, false, null, "FRANK.HALL@EXAMPLE.COM", "FRANKTHEMAN", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "", false, "FrankTheMan" }
                });

            migrationBuilder.InsertData(
                table: "Connections",
                columns: new[] { "id", "Accepted", "CreatedAt", "FriendId", "UserId" },
                values: new object[,]
                {
                    { new Guid("60f01455-f4f7-4986-9489-75ffd3d699e0"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "c1f3d713-5678-43fa-9c8e-65fa6ee39245", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("a875b4fb-f2b1-4a68-8684-4ab4915c002d"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "d1f4d713-9101-43fa-9c8e-65fa6ee39246", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("bbb967e5-a6b5-41c0-9d69-6a72cc05a12f"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "e1f5d713-1122-43fa-9c8e-65fa6ee39247", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("cfa42098-b22a-4272-b9d9-52f4b57de616"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "f1f6d713-3344-43fa-9c8e-65fa6ee39248", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("dcdf519b-70a5-448a-8599-515d9da297d9"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "a1f2d713-1234-43fa-9c8e-65fa6ee39244", "bb29d713-9414-43fa-9c8e-65fa6ee39243" }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name", "OwnerId" },
                values: new object[,]
                {
                    { new Guid("25d5ec2b-ebe4-4462-ada9-17c246fb5273"), "Group 2", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("707e730d-0d72-4109-b9d9-5dc47b637268"), "Group 4", "d1f4d713-9101-43fa-9c8e-65fa6ee39246" },
                    { new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Group 1", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("c7fcbe94-0f5f-47e6-9b71-5cc04ce32538"), "Group 5", "e1f5d713-1122-43fa-9c8e-65fa6ee39247" },
                    { new Guid("f50053c8-7fd8-498b-8c0b-30277bc378b0"), "Group 3", "c1f3d713-5678-43fa-9c8e-65fa6ee39245" }
                });

            migrationBuilder.InsertData(
                table: "GroupRoles",
                columns: new[] { "Id", "Color", "GroupId", "Name" },
                values: new object[,]
                {
                    { new Guid("62a70d41-0339-46f1-81c3-6d27d9cda762"), "64bcff", new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Moderator" },
                    { new Guid("927af184-f6bc-4d8a-b36e-ff5f8aa3d14b"), "64bcff", new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Kung för en dag" },
                    { new Guid("eec0a883-0fb8-4f7a-bea7-04892684b1bd"), "64bcff", new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Boss" },
                    { new Guid("f3dc9330-dce9-4bfc-9844-dd8232fce023"), "64bcff", new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Administrator" }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content", "CreatedAt", "GroupId", "UserId" },
                values: new object[,]
                {
                    { new Guid("1baf7663-c7da-4954-bd1c-2865de582301"), "Nämen tjena", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "a1f2d713-1234-43fa-9c8e-65fa6ee39244" },
                    { new Guid("7acbca3e-d27a-403c-af5b-37b31e4bf53a"), "Hej", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" }
                });

            migrationBuilder.InsertData(
                table: "UserGroups",
                columns: new[] { "GroupsId", "UsersId" },
                values: new object[,]
                {
                    { new Guid("25d5ec2b-ebe4-4462-ada9-17c246fb5273"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("707e730d-0d72-4109-b9d9-5dc47b637268"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "a1f2d713-1234-43fa-9c8e-65fa6ee39244" },
                    { new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("c7fcbe94-0f5f-47e6-9b71-5cc04ce32538"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("f50053c8-7fd8-498b-8c0b-30277bc378b0"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" }
                });

            migrationBuilder.InsertData(
                table: "UserGroupRoles",
                columns: new[] { "Id", "GroupRoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("27c9c624-d28d-47f8-b875-9502b5522cc7"), new Guid("eec0a883-0fb8-4f7a-bea7-04892684b1bd"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("3c69703b-48e1-44e0-8bab-6f8d7cf8d41c"), new Guid("927af184-f6bc-4d8a-b36e-ff5f8aa3d14b"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("4fef1968-54e7-4352-b7dc-52c9e9d223a4"), new Guid("f3dc9330-dce9-4bfc-9844-dd8232fce023"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("fc1a0e5c-e610-4929-b18e-b25324121a5d"), new Guid("62a70d41-0339-46f1-81c3-6d27d9cda762"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Connections_FriendId",
                table: "Connections",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_UserId",
                table: "Connections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_GroupId",
                table: "GroupRoles",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_GroupId",
                table: "Invites",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_UserId",
                table: "Invites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_GroupId",
                table: "Messages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupRoles_GroupRoleId",
                table: "UserGroupRoles",
                column: "GroupRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupRoles_UserId",
                table: "UserGroupRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UsersId",
                table: "UserGroups",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "Invites");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "UserGroupRoles");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "GroupRoles");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
