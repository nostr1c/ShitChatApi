using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class addingGroupRoleColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "GroupRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("62a70d41-0339-46f1-81c3-6d27d9cda762"),
                column: "Color",
                value: "64bcff");

            migrationBuilder.UpdateData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("927af184-f6bc-4d8a-b36e-ff5f8aa3d14b"),
                column: "Color",
                value: "64bcff");

            migrationBuilder.UpdateData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("eec0a883-0fb8-4f7a-bea7-04892684b1bd"),
                column: "Color",
                value: "64bcff");

            migrationBuilder.UpdateData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("f3dc9330-dce9-4bfc-9844-dd8232fce023"),
                column: "Color",
                value: "64bcff");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "GroupRoles");
        }
    }
}
