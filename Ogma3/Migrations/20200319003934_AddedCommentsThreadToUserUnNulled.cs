using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedCommentsThreadToUserUnNulled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CommentsThreadId",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CommentsThreadId",
                table: "AspNetUsers",
                column: "CommentsThreadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CommentsThreadId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "CommentsThreadId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
