using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGroupRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "UserGroupRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 27));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 27));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 27));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 27));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 27));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 27));

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_GroupId",
                table: "GroupRoles",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupRoles_GroupRoleId",
                table: "UserGroupRoles",
                column: "GroupRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupRoles_UserId",
                table: "UserGroupRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroupRoles");

            migrationBuilder.DropTable(
                name: "GroupRoles");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 5));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 5));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 5));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 5));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 5));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 5));
        }
    }
}
