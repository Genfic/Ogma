using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedAuthorToStoryEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Stories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_AuthorId",
                table: "Stories",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_AspNetUsers_AuthorId",
                table: "Stories",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_AspNetUsers_AuthorId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_AuthorId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Stories");
        }
    }
}
