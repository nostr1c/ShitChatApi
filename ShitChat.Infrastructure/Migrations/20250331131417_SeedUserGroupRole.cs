using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserGroupRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 31));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 31));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 31));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 31));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 31));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
                column: "CreatedAt",
                value: new DateOnly(2025, 3, 31));

            migrationBuilder.InsertData(
                table: "GroupRoles",
                columns: new[] { "Id", "GroupId", "Name" },
                values: new object[,]
                {
                    { new Guid("62a70d41-0339-46f1-81c3-6d27d9cda762"), new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Moderator" },
                    { new Guid("927af184-f6bc-4d8a-b36e-ff5f8aa3d14b"), new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Kung för en dag" },
                    { new Guid("eec0a883-0fb8-4f7a-bea7-04892684b1bd"), new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Boss" },
                    { new Guid("f3dc9330-dce9-4bfc-9844-dd8232fce023"), new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "Administrator" }
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserGroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("27c9c624-d28d-47f8-b875-9502b5522cc7"));

            migrationBuilder.DeleteData(
                table: "UserGroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("3c69703b-48e1-44e0-8bab-6f8d7cf8d41c"));

            migrationBuilder.DeleteData(
                table: "UserGroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("4fef1968-54e7-4352-b7dc-52c9e9d223a4"));

            migrationBuilder.DeleteData(
                table: "UserGroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("fc1a0e5c-e610-4929-b18e-b25324121a5d"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("62a70d41-0339-46f1-81c3-6d27d9cda762"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("927af184-f6bc-4d8a-b36e-ff5f8aa3d14b"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("eec0a883-0fb8-4f7a-bea7-04892684b1bd"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("f3dc9330-dce9-4bfc-9844-dd8232fce023"));

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
        }
    }
}
