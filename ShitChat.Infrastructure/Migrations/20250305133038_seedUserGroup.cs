using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class seedUserGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "UserGroups",
                columns: new[] { "GroupsId", "UsersId" },
                values: new object[] { new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "a1f2d713-1234-43fa-9c8e-65fa6ee39244" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserGroups",
                keyColumns: new[] { "GroupsId", "UsersId" },
                keyValues: new object[] { new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "a1f2d713-1234-43fa-9c8e-65fa6ee39244" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 15));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 15));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 15));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 15));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 15));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 15));
        }
    }
}
