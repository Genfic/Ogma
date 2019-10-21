using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedSlugs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_Stories_StoryId",
                table: "StoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_Tags_TagId",
                table: "StoryTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryTag",
                table: "StoryTag");

            migrationBuilder.RenameTable(
                name: "StoryTag",
                newName: "StoryTags");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTag_TagId",
                table: "StoryTags",
                newName: "IX_StoryTags_TagId");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Stories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Chapters",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryTags",
                table: "StoryTags",
                columns: new[] { "StoryId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_Stories_StoryId",
                table: "StoryTags",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_Tags_TagId",
                table: "StoryTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_Stories_StoryId",
                table: "StoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_Tags_TagId",
                table: "StoryTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryTags",
                table: "StoryTags");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Chapters");

            migrationBuilder.RenameTable(
                name: "StoryTags",
                newName: "StoryTag");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTags_TagId",
                table: "StoryTag",
                newName: "IX_StoryTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryTag",
                table: "StoryTag",
                columns: new[] { "StoryId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_Stories_StoryId",
                table: "StoryTag",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_Tags_TagId",
                table: "StoryTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
