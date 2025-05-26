using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShitChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243",
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
