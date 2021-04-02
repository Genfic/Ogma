using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Ogma3.Data.Stories;

namespace Ogma3.Migrations
{
    public partial class ExternalConfigTest_Stories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WordCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<EStoryStatus>(
                name: "Status",
                table: "Stories",
                type: "e_story_status",
                nullable: false,
                defaultValue: EStoryStatus.InProgress,
                oldClrType: typeof(EStoryStatus),
                oldType: "e_story_status");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Stories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "ChapterCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WordCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<EStoryStatus>(
                name: "Status",
                table: "Stories",
                type: "e_story_status",
                nullable: false,
                oldClrType: typeof(EStoryStatus),
                oldType: "e_story_status",
                oldDefaultValue: EStoryStatus.InProgress);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Stories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "ChapterCount",
                table: "Stories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);
        }
    }
}
