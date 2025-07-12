using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class BetterRatingIcons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconId",
                table: "Ratings");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Ratings",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Ratings",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Ratings");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Ratings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconId",
                table: "Ratings",
                type: "text",
                nullable: true);
        }
    }
}
