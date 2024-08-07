using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class BetterIndexesOnInfractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Infractions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Infractions_RemovedAt",
                table: "Infractions",
                column: "RemovedAt",
                filter: "\"RemovedAt\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Infractions_Type",
                table: "Infractions",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Infractions_RemovedAt",
                table: "Infractions");

            migrationBuilder.DropIndex(
                name: "IX_Infractions_Type",
                table: "Infractions");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Infractions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);
        }
    }
}
