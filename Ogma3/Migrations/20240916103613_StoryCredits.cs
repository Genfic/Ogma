using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class StoryCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Credits",
                table: "Stories",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credits",
                table: "Stories");
        }
    }
}
