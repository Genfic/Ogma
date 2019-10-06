using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class ChangedConstraintsForStoryAndChapterEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 18, 42, 24, 86, DateTimeKind.Local).AddTicks(442),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2019, 10, 6, 18, 31, 19, 260, DateTimeKind.Local).AddTicks(138));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Stories",
                maxLength: 1500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "Hook",
                table: "Stories",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDate",
                table: "Chapters",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 18, 42, 24, 91, DateTimeKind.Local).AddTicks(5772),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "EndNotes",
                table: "Chapters",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartNotes",
                table: "Chapters",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hook",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "EndNotes",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "StartNotes",
                table: "Chapters");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Stories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 6, 18, 31, 19, 260, DateTimeKind.Local).AddTicks(138),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 6, 18, 42, 24, 86, DateTimeKind.Local).AddTicks(442));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Stories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDate",
                table: "Chapters",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 6, 18, 42, 24, 91, DateTimeKind.Local).AddTicks(5772));
        }
    }
}
