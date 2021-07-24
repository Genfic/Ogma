using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ogma3.Migrations
{
    public partial class ChapterReadsAndUnreads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChaptersRead",
                table: "ChaptersRead");

            migrationBuilder.DropIndex(
                name: "IX_ChaptersRead_StoryId",
                table: "ChaptersRead");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChaptersRead");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChaptersRead",
                table: "ChaptersRead",
                columns: new[] { "StoryId", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChaptersRead",
                table: "ChaptersRead");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ChaptersRead",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChaptersRead",
                table: "ChaptersRead",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChaptersRead_StoryId",
                table: "ChaptersRead",
                column: "StoryId");
        }
    }
}
