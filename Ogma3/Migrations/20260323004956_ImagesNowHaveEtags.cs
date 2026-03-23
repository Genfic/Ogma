using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class ImagesNowHaveEtags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackblazeId",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "ETag",
                table: "Images",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETag",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "BackblazeId",
                table: "Images",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
