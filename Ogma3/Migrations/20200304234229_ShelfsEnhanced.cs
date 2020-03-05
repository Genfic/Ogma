using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class ShelfsEnhanced : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shelves",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "Argb",
                table: "Shelves",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shelves",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuickAdd",
                table: "Shelves",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Argb",
                table: "Shelves");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shelves");

            migrationBuilder.DropColumn(
                name: "IsQuickAdd",
                table: "Shelves");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shelves",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
