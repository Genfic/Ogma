using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ogma3.Migrations
{
    public partial class ContentBlocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContentBlockId",
                table: "Stories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContentBlockId",
                table: "Chapters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContentBlockId",
                table: "Blogposts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContentBlock",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssuerId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentBlock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentBlock_AspNetUsers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_ContentBlockId",
                table: "Stories",
                column: "ContentBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_ContentBlockId",
                table: "Chapters",
                column: "ContentBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_ContentBlockId",
                table: "Blogposts",
                column: "ContentBlockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlock_IssuerId",
                table: "ContentBlock",
                column: "IssuerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogposts_ContentBlock_ContentBlockId",
                table: "Blogposts",
                column: "ContentBlockId",
                principalTable: "ContentBlock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_ContentBlock_ContentBlockId",
                table: "Chapters",
                column: "ContentBlockId",
                principalTable: "ContentBlock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_ContentBlock_ContentBlockId",
                table: "Stories",
                column: "ContentBlockId",
                principalTable: "ContentBlock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogposts_ContentBlock_ContentBlockId",
                table: "Blogposts");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_ContentBlock_ContentBlockId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_ContentBlock_ContentBlockId",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "ContentBlock");

            migrationBuilder.DropIndex(
                name: "IX_Stories_ContentBlockId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_ContentBlockId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Blogposts_ContentBlockId",
                table: "Blogposts");

            migrationBuilder.DropColumn(
                name: "ContentBlockId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ContentBlockId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "ContentBlockId",
                table: "Blogposts");
        }
    }
}
