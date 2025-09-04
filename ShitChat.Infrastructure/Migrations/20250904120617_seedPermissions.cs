using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShitChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("037f49f3-9f9f-4c45-b94b-1b8c0e595fb9"), "manage_invites" },
                    { new Guid("47d3b3f7-2e55-4867-9bca-4e1f971ae5ae"), "manage_server" },
                    { new Guid("5a6ba92e-1013-4c73-9589-0dba08bfa2bf"), "delete_messages" },
                    { new Guid("61e895bb-021f-42ae-88af-c7444931630e"), "manage_server_roles" },
                    { new Guid("bd74b2af-ed25-48a5-8ab4-f78227a58d06"), "kick_user" },
                    { new Guid("c8161caf-eb44-4c71-baf1-eea17481989c"), "manage_user_roles" },
                    { new Guid("e5bebdec-1a32-4c9d-a54e-39cc0d073ed6"), "ban_user" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("037f49f3-9f9f-4c45-b94b-1b8c0e595fb9"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("47d3b3f7-2e55-4867-9bca-4e1f971ae5ae"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("5a6ba92e-1013-4c73-9589-0dba08bfa2bf"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("61e895bb-021f-42ae-88af-c7444931630e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("bd74b2af-ed25-48a5-8ab4-f78227a58d06"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("c8161caf-eb44-4c71-baf1-eea17481989c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("e5bebdec-1a32-4c9d-a54e-39cc0d073ed6"));
        }
    }
}
