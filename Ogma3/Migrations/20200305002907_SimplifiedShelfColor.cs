using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class SimplifiedShelfColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Argb",
                table: "Shelves");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Shelves",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Shelves");

            migrationBuilder.AddColumn<long>(
                name: "Argb",
                table: "Shelves",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
