using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class YeetedTheIconFieldFromRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Ratings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Ratings",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }
    }
}
