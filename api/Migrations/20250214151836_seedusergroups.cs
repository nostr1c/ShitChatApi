using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class seedusergroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 14));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 14));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 14));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 14));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 14));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
                column: "CreatedAt",
                value: new DateOnly(2025, 2, 14));

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
                table: "UserGroups",
                columns: new[] { "GroupsId", "UsersId" },
                values: new object[,]
                {
                    { new Guid("25d5ec2b-ebe4-4462-ada9-17c246fb5273"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("707e730d-0d72-4109-b9d9-5dc47b637268"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("c7fcbe94-0f5f-47e6-9b71-5cc04ce32538"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" },
                    { new Guid("f50053c8-7fd8-498b-8c0b-30277bc378b0"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserGroups",
                keyColumns: new[] { "GroupsId", "UsersId" },
                keyValues: new object[] { new Guid("25d5ec2b-ebe4-4462-ada9-17c246fb5273"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" });

            migrationBuilder.DeleteData(
                table: "UserGroups",
                keyColumns: new[] { "GroupsId", "UsersId" },
                keyValues: new object[] { new Guid("707e730d-0d72-4109-b9d9-5dc47b637268"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" });

            migrationBuilder.DeleteData(
                table: "UserGroups",
                keyColumns: new[] { "GroupsId", "UsersId" },
                keyValues: new object[] { new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" });

            migrationBuilder.DeleteData(
                table: "UserGroups",
                keyColumns: new[] { "GroupsId", "UsersId" },
                keyValues: new object[] { new Guid("c7fcbe94-0f5f-47e6-9b71-5cc04ce32538"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" });

            migrationBuilder.DeleteData(
                table: "UserGroups",
                keyColumns: new[] { "GroupsId", "UsersId" },
                keyValues: new object[] { new Guid("f50053c8-7fd8-498b-8c0b-30277bc378b0"), "bb29d713-9414-43fa-9c8e-65fa6ee39243" });

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("25d5ec2b-ebe4-4462-ada9-17c246fb5273"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("707e730d-0d72-4109-b9d9-5dc47b637268"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("be081304-63c6-4cae-bf25-b7e33cc6e495"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("c7fcbe94-0f5f-47e6-9b71-5cc04ce32538"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("f50053c8-7fd8-498b-8c0b-30277bc378b0"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244",
                column: "CreatedAt",
                value: new DateOnly(2025, 1, 20));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243",
                column: "CreatedAt",
                value: new DateOnly(2025, 1, 20));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245",
                column: "CreatedAt",
                value: new DateOnly(2025, 1, 20));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246",
                column: "CreatedAt",
                value: new DateOnly(2025, 1, 20));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247",
                column: "CreatedAt",
                value: new DateOnly(2025, 1, 20));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248",
                column: "CreatedAt",
                value: new DateOnly(2025, 1, 20));
        }
    }
}
