using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShitChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addLastActivityToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivity",
                table: "Groups",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "Groups");
        }
    }
}
