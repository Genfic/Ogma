using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedCommentsThreadToUserUnNulled2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
