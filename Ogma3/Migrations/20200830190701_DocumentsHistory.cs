using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class DocumentsHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRevision",
                table: "Documents");

            migrationBuilder.AddColumn<DateTime>(
                name: "RevisionDate",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Documents",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevisionDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Documents");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRevision",
                table: "Documents",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
