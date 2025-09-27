using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShitChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addMessageAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageAttachment_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttachment_FileName",
                table: "MessageAttachment",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttachment_MessageId",
                table: "MessageAttachment",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageAttachment");
        }
    }
}
