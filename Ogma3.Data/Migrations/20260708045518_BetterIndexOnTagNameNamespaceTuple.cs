using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260708045518_BetterIndexOnTagNameNamespaceTuple : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Tags_Name_Namespace",
            table: "Tags");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name_Namespace",
            table: "Tags",
            columns: new[] { "Name", "Namespace" },
            unique: true)
            .Annotation("Npgsql:NullsDistinct", false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Tags_Name_Namespace",
            table: "Tags");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name_Namespace",
            table: "Tags",
            columns: new[] { "Name", "Namespace" },
            unique: true);
    }
}
