using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class UnidirectionalRelationshipTet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "Tags",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    Cover = table.Column<string>(nullable: true),
                    CoverId = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2019, 10, 6, 3, 4, 52, 630, DateTimeKind.Local).AddTicks(9245)),
                    Rating = table.Column<string>(nullable: false, defaultValue: "Everyone")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Stories_StoryId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Tags_StoryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Tags");
        }
    }
}
