using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    public partial class WhoAddedStoryToFolderAndWhen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Added",
                table: "FolderStories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<long>(
                name: "AddedById",
                table: "FolderStories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);

            migrationBuilder.CreateIndex(
                name: "IX_FolderStories_AddedById",
                table: "FolderStories",
                column: "AddedById");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderStories_AspNetUsers_AddedById",
                table: "FolderStories",
                column: "AddedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderStories_AspNetUsers_AddedById",
                table: "FolderStories");

            migrationBuilder.DropIndex(
                name: "IX_FolderStories_AddedById",
                table: "FolderStories");

            migrationBuilder.DropColumn(
                name: "Added",
                table: "FolderStories");

            migrationBuilder.DropColumn(
                name: "AddedById",
                table: "FolderStories");

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);
        }
    }
}
