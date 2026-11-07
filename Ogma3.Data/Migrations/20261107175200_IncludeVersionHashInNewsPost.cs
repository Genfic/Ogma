using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20261107175200_IncludeVersionHashInNewsPost : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CommitHash",
            table: "News",
            type: "character varying(64)",
            maxLength: 64,
            nullable: false,
            defaultValue: "CAFEBABE");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CommitHash",
            table: "News");
    }
}
