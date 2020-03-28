using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedBlogpostComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentsThreadId",
                table: "Blogposts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts",
                column: "CommentsThreadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogposts_CommentThreads_CommentsThreadId",
                table: "Blogposts",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogposts_CommentThreads_CommentsThreadId",
                table: "Blogposts");

            migrationBuilder.DropIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts");

            migrationBuilder.DropColumn(
                name: "CommentsThreadId",
                table: "Blogposts");
        }
    }
}
