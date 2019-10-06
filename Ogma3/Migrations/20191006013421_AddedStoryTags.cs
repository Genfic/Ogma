using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class AddedStoryTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Stories_StoryId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_StoryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Tags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 3, 34, 20, 751, DateTimeKind.Local).AddTicks(996),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2019, 10, 6, 3, 4, 52, 630, DateTimeKind.Local).AddTicks(9245));

            migrationBuilder.CreateTable(
                name: "StoryTag",
                columns: table => new
                {
                    StoryId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTag", x => new { x.StoryId, x.TagId });
                    table.ForeignKey(
                        name: "FK_StoryTag_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoryTag_TagId",
                table: "StoryTag",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryTag");

            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 3, 4, 52, 630, DateTimeKind.Local).AddTicks(9245),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 6, 3, 34, 20, 751, DateTimeKind.Local).AddTicks(996));

            migrationBuilder.CreateIndex(
                name: "IX_Tags_StoryId",
                table: "Tags",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Stories_StoryId",
                table: "Tags",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
