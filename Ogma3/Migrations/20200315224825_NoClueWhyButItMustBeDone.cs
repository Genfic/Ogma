using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class NoClueWhyButItMustBeDone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IconId",
                table: "Shelves",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_IconId",
                table: "Shelves",
                column: "IconId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Icons_IconId",
                table: "Shelves",
                column: "IconId",
                principalTable: "Icons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Icons_IconId",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_Shelves_IconId",
                table: "Shelves");

            migrationBuilder.DropColumn(
                name: "IconId",
                table: "Shelves");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Shelves",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
