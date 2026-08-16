using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260816204948_TosAndPpAcceptance : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "PrivacyPolicyAcceptedAt",
            table: "AspNetUsers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "TosAcceptedAt",
            table: "AspNetUsers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PrivacyPolicyAcceptedAt",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "TosAcceptedAt",
            table: "AspNetUsers");
    }
}
