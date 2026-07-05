using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260705021134_DenormalizedStoryVoteCountAndLastUpdateDate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "LastUpdatedAt",
            table: "Stories",
            type: "timestamp with time zone",
            nullable: true,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<int>(
            name: "VoteCount",
            table: "Stories",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LastUpdatedAt",
            table: "Stories");

        migrationBuilder.DropColumn(
            name: "VoteCount",
            table: "Stories");
    }
}
