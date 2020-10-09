using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class BetterDocRevisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Documents");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Slug_Version",
                table: "Documents",
                columns: new[] { "Slug", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Title_Version",
                table: "Documents",
                columns: new[] { "Title", "Version" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documents_Slug_Version",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_Title_Version",
                table: "Documents");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Documents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
