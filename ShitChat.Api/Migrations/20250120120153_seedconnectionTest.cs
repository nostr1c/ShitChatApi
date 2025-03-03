using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class seedconnectionTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Connections",
                columns: new[] { "id", "Accepted", "CreatedAt", "FriendId", "UserId" },
                values: new object[] { new Guid("dcdf519b-70a5-448a-8599-515d9da297d9"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "a1f2d713-1234-43fa-9c8e-65fa6ee39244", "bb29d713-9414-43fa-9c8e-65fa6ee39243" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Connections",
                keyColumn: "id",
                keyValue: new Guid("dcdf519b-70a5-448a-8599-515d9da297d9"));
        }
    }
}
