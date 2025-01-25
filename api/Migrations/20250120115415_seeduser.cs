using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class seeduser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AvatarUri", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "a1f2d713-1234-43fa-9c8e-65fa6ee39244", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 1, 20), "bob.jones@example.com", true, false, null, "BOB.JONES@EXAMPLE.COM", "BOBLORD1337", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "BobLord1337" },
                    { "bb29d713-9414-43fa-9c8e-65fa6ee39243", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 1, 20), "alice.smith@example.com", true, false, null, "ALICE.SMITH@EXAMPLE.COM", "ALICE123", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "alice123" },
                    { "c1f3d713-5678-43fa-9c8e-65fa6ee39245", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 1, 20), "carla.davis@example.com", true, false, null, "CARLA.DAVIS@EXAMPLE.COM", "CARLIS", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "Carlis" },
                    { "d1f4d713-9101-43fa-9c8e-65fa6ee39246", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 1, 20), "david.lee@example.com", true, false, null, "DAVID.LEE@EXAMPLE.COM", "DAVIDLEE", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "DavidLee" },
                    { "e1f5d713-1122-43fa-9c8e-65fa6ee39247", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 1, 20), "emily.white@example.com", true, false, null, "EMILY.WHITE@EXAMPLE.COM", "EMILYCOOL", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "EmilyCool" },
                    { "f1f6d713-3344-43fa-9c8e-65fa6ee39248", 0, "a2138670-ffb4-466c-b40c-44dde76566ed.jpg", null, new DateOnly(2025, 1, 20), "frank.hall@example.com", true, false, null, "FRANK.HALL@EXAMPLE.COM", "FRANKTHEMAN", "AQAAAAIAAYagAAAAEG719JW0JH1H9VQgj8uzgJ4HLJ+/2qP7NjjLeDMIku1+rtQT16BvU3uracoab0E0Gg==", null, false, "exampletoken1", new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "FrankTheMan" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1f2d713-1234-43fa-9c8e-65fa6ee39244");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bb29d713-9414-43fa-9c8e-65fa6ee39243");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f3d713-5678-43fa-9c8e-65fa6ee39245");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d1f4d713-9101-43fa-9c8e-65fa6ee39246");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e1f5d713-1122-43fa-9c8e-65fa6ee39247");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1f6d713-3344-43fa-9c8e-65fa6ee39248");
        }
    }
}
