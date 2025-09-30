using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShitChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class cascadeDelUserGroupRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupRoles_GroupRoles_GroupRoleId",
                table: "UserGroupRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupRoles_GroupRoles_GroupRoleId",
                table: "UserGroupRoles",
                column: "GroupRoleId",
                principalTable: "GroupRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupRoles_GroupRoles_GroupRoleId",
                table: "UserGroupRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupRoles_GroupRoles_GroupRoleId",
                table: "UserGroupRoles",
                column: "GroupRoleId",
                principalTable: "GroupRoles",
                principalColumn: "Id");
        }
    }
}
