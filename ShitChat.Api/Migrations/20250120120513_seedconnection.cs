using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class seedconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Connections",
                columns: new[] { "id", "Accepted", "CreatedAt", "FriendId", "UserId" },
                values: new object[,]
                {
                    { new Guid("60f01455-f4f7-4986-9489-75ffd3d699e0"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "c1f3d713-5678-43fa-9c8e-65fa6ee39245", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("a875b4fb-f2b1-4a68-8684-4ab4915c002d"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "d1f4d713-9101-43fa-9c8e-65fa6ee39246", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("bbb967e5-a6b5-41c0-9d69-6a72cc05a12f"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "e1f5d713-1122-43fa-9c8e-65fa6ee39247", "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("cfa42098-b22a-4272-b9d9-52f4b57de616"), true, new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "f1f6d713-3344-43fa-9c8e-65fa6ee39248", "bb29d713-9414-43fa-9c8e-65fa6ee39243" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Connections",
                keyColumn: "id",
                keyValue: new Guid("60f01455-f4f7-4986-9489-75ffd3d699e0"));

            migrationBuilder.DeleteData(
                table: "Connections",
                keyColumn: "id",
                keyValue: new Guid("a875b4fb-f2b1-4a68-8684-4ab4915c002d"));

            migrationBuilder.DeleteData(
                table: "Connections",
                keyColumn: "id",
                keyValue: new Guid("bbb967e5-a6b5-41c0-9d69-6a72cc05a12f"));

            migrationBuilder.DeleteData(
                table: "Connections",
                keyColumn: "id",
                keyValue: new Guid("cfa42098-b22a-4272-b9d9-52f4b57de616"));
        }
    }
}
