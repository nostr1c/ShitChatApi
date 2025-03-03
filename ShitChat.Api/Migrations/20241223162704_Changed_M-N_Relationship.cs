using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Changed_MN_Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_AspNetUsers_UserId",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.RenameTable(
                name: "UserGroups",
                newName: "UserGroup");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroup",
                newName: "IX_UserGroup_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroup",
                table: "UserGroup",
                columns: new[] { "UserId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroup_AspNetUsers_UserId",
                table: "UserGroup",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroup_Groups_GroupId",
                table: "UserGroup",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroup_AspNetUsers_UserId",
                table: "UserGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroup_Groups_GroupId",
                table: "UserGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroup",
                table: "UserGroup");

            migrationBuilder.RenameTable(
                name: "UserGroup",
                newName: "UserGroups");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroup_GroupId",
                table: "UserGroups",
                newName: "IX_UserGroups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                columns: new[] { "UserId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_AspNetUsers_UserId",
                table: "UserGroups",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
