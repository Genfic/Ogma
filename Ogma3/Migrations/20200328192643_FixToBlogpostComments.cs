using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class FixToBlogpostComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts");

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts",
                column: "CommentsThreadId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts");

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts",
                column: "CommentsThreadId");
        }
    }
}
