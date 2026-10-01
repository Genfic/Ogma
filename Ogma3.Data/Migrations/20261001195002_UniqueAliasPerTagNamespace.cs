using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20261001195002_UniqueAliasPerTagNamespace : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_TagNamespaces_Alias",
            table: "TagNamespaces",
            column: "Alias",
            unique: true,
            filter: "\"Alias\" IS NOT NULL")
            .Annotation("Relational:Collation", new[] { "nocase-noaccent" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_TagNamespaces_Alias",
            table: "TagNamespaces");
    }
}
