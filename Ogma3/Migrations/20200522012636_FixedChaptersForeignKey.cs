using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class FixedChaptersForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "StoryId",
                table: "Chapters",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Stories_StoryId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_StoryId",
                table: "Chapters");

            migrationBuilder.AlterColumn<int>(
                name: "StoryId",
                table: "Chapters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "StoryId1",
                table: "Chapters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_StoryId1",
                table: "Chapters",
                column: "StoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Stories_StoryId1",
                table: "Chapters",
                column: "StoryId1",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
