using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class BlogpostAttachedStoriesAndChapters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AttachedChapterId",
                table: "Blogposts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AttachedStoryId",
                table: "Blogposts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_AttachedChapterId",
                table: "Blogposts",
                column: "AttachedChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_AttachedStoryId",
                table: "Blogposts",
                column: "AttachedStoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogposts_Chapters_AttachedChapterId",
                table: "Blogposts",
                column: "AttachedChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogposts_Stories_AttachedStoryId",
                table: "Blogposts",
                column: "AttachedStoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogposts_Chapters_AttachedChapterId",
                table: "Blogposts");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogposts_Stories_AttachedStoryId",
                table: "Blogposts");

            migrationBuilder.DropIndex(
                name: "IX_Blogposts_AttachedChapterId",
                table: "Blogposts");

            migrationBuilder.DropIndex(
                name: "IX_Blogposts_AttachedStoryId",
                table: "Blogposts");

            migrationBuilder.DropColumn(
                name: "AttachedChapterId",
                table: "Blogposts");

            migrationBuilder.DropColumn(
                name: "AttachedStoryId",
                table: "Blogposts");
        }
    }
}
