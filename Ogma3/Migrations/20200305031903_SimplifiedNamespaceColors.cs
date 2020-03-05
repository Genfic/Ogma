using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class SimplifiedNamespaceColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Argb",
                table: "Namespaces");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Shelves",
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Namespaces",
                maxLength: 7,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Namespaces");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Shelves",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 7,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Argb",
                table: "Namespaces",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
