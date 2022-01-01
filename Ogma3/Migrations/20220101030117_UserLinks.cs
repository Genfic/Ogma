using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    public partial class UserLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<List<string>>(
                name: "Links",
                table: "AspNetUsers",
                type: "text[]",
                maxLength: 5,
                nullable: false,
                defaultValueSql: "'{}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Links",
                table: "AspNetUsers");

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
