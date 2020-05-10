using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class UpdatedMoreForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShelfStories_Shelves_ShelfId1",
                table: "ShelfStories");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfStories_Stories_StoryId1",
                table: "ShelfStories");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Icons_IconId1",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_Shelves_IconId1",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_ShelfStories_ShelfId1",
                table: "ShelfStories");

            migrationBuilder.DropIndex(
                name: "IX_ShelfStories_StoryId1",
                table: "ShelfStories");

            migrationBuilder.DropColumn(
                name: "IconId1",
                table: "Shelves");

            migrationBuilder.DropColumn(
                name: "ShelfId1",
                table: "ShelfStories");

            migrationBuilder.DropColumn(
                name: "StoryId1",
                table: "ShelfStories");

            migrationBuilder.AlterColumn<long>(
                name: "IconId",
                table: "Shelves",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StoryId",
                table: "ShelfStories",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "ShelfId",
                table: "ShelfStories",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_IconId",
                table: "Shelves",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfStories_StoryId",
                table: "ShelfStories",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfStories_Shelves_ShelfId",
                table: "ShelfStories",
                column: "ShelfId",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfStories_Stories_StoryId",
                table: "ShelfStories",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Icons_IconId",
                table: "Shelves",
                column: "IconId",
                principalTable: "Icons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShelfStories_Shelves_ShelfId",
                table: "ShelfStories");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfStories_Stories_StoryId",
                table: "ShelfStories");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Icons_IconId",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_Shelves_IconId",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_ShelfStories_StoryId",
                table: "ShelfStories");

            migrationBuilder.AlterColumn<int>(
                name: "IconId",
                table: "Shelves",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IconId1",
                table: "Shelves",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StoryId",
                table: "ShelfStories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "ShelfId",
                table: "ShelfStories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "ShelfId1",
                table: "ShelfStories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StoryId1",
                table: "ShelfStories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_IconId1",
                table: "Shelves",
                column: "IconId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfStories_ShelfId1",
                table: "ShelfStories",
                column: "ShelfId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfStories_StoryId1",
                table: "ShelfStories",
                column: "StoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfStories_Shelves_ShelfId1",
                table: "ShelfStories",
                column: "ShelfId1",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfStories_Stories_StoryId1",
                table: "ShelfStories",
                column: "StoryId1",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Icons_IconId1",
                table: "Shelves",
                column: "IconId1",
                principalTable: "Icons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
