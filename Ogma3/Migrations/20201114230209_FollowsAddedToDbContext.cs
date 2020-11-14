using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class FollowsAddedToDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_AspNetUsers_FollowedUserId",
                table: "UserFollow");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_AspNetUsers_FollowingUserId",
                table: "UserFollow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollow",
                table: "UserFollow");

            migrationBuilder.RenameTable(
                name: "UserFollow",
                newName: "FollowedUsers");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollow_FollowedUserId",
                table: "FollowedUsers",
                newName: "IX_FollowedUsers_FollowedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FollowedUsers",
                table: "FollowedUsers",
                columns: new[] { "FollowingUserId", "FollowedUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FollowedUsers_AspNetUsers_FollowedUserId",
                table: "FollowedUsers",
                column: "FollowedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowedUsers_AspNetUsers_FollowingUserId",
                table: "FollowedUsers",
                column: "FollowingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowedUsers_AspNetUsers_FollowedUserId",
                table: "FollowedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowedUsers_AspNetUsers_FollowingUserId",
                table: "FollowedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FollowedUsers",
                table: "FollowedUsers");

            migrationBuilder.RenameTable(
                name: "FollowedUsers",
                newName: "UserFollow");

            migrationBuilder.RenameIndex(
                name: "IX_FollowedUsers_FollowedUserId",
                table: "UserFollow",
                newName: "IX_UserFollow_FollowedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollow",
                table: "UserFollow",
                columns: new[] { "FollowingUserId", "FollowedUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_AspNetUsers_FollowedUserId",
                table: "UserFollow",
                column: "FollowedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_AspNetUsers_FollowingUserId",
                table: "UserFollow",
                column: "FollowingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
