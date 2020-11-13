using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class BetterUserBlockStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_UserId",
                table: "BlacklistedUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BlacklistedUsers",
                newName: "BlockingUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_BlockingUserId",
                table: "BlacklistedUsers",
                column: "BlockingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_BlockingUserId",
                table: "BlacklistedUsers");

            migrationBuilder.RenameColumn(
                name: "BlockingUserId",
                table: "BlacklistedUsers",
                newName: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_UserId",
                table: "BlacklistedUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
