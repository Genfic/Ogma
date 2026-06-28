using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260628080335_YeetedEntitlementsColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Entitlements",
            table: "SubscriptionTiers");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int[]>(
            name: "Entitlements",
            table: "SubscriptionTiers",
            type: "integer[]",
            nullable: false,
            defaultValue: new int[0]);
    }
}
