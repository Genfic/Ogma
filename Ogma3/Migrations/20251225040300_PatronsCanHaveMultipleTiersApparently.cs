using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class PatronsCanHaveMultipleTiersApparently : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatreonTierId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<List<string>>(
                name: "PatreonTierIds",
                table: "Subscriptions",
                type: "text[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatreonTierIds",
                table: "Subscriptions");

            migrationBuilder.AddColumn<string>(
                name: "PatreonTierId",
                table: "Subscriptions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
