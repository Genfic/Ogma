using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class CommentsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentThreads_CommentsThreadId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentsThreadId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentsThreadId1",
                table: "Comments");

            migrationBuilder.AlterColumn<long>(
                name: "CommentsThreadId",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentsThreadId",
                table: "Comments",
                column: "CommentsThreadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_CommentThreads_CommentsThreadId",
                table: "Comments",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentThreads_CommentsThreadId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentsThreadId",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "CommentsThreadId",
                table: "Comments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "CommentsThreadId1",
                table: "Comments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentsThreadId1",
                table: "Comments",
                column: "CommentsThreadId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_CommentThreads_CommentsThreadId1",
                table: "Comments",
                column: "CommentsThreadId1",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
