using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedCommentThreadsToChapters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentsThreadId",
                table: "Chapters",
                nullable: true,
                type: "int"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CommentsThreadId",
                table: "Chapters",
                column: "CommentsThreadId",
                unique: true,
                filter: "[CommentsThreadId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chapters_CommentsThreadId",
                table: "Chapters");

            migrationBuilder.AlterColumn<int>(
                name: "CommentsThreadId",
                table: "Chapters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
