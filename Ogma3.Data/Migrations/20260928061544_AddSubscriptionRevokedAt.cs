using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260928061544_AddSubscriptionRevokedAt : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "RevokedAt",
            table: "Subscriptions",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Subscriptions_RevokedAt",
            table: "Subscriptions",
            column: "RevokedAt");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Subscriptions_RevokedAt",
            table: "Subscriptions");

        migrationBuilder.DropColumn(
            name: "RevokedAt",
            table: "Subscriptions");
    }
}
