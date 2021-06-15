using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class ClubThreadPinning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "ClubThreads",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "ClubThreads");

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
