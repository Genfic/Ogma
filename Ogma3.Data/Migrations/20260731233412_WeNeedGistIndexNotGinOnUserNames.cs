using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260731233412_WeNeedGistIndexNotGinOnUserNames : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_NormalizedUserName_Trgm",
            table: "AspNetUsers");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_NormalizedUserName_Trgm",
            table: "AspNetUsers",
            column: "NormalizedUserName")
            .Annotation("Npgsql:IndexMethod", "gist")
            .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_NormalizedUserName_Trgm",
            table: "AspNetUsers");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_NormalizedUserName_Trgm",
            table: "AspNetUsers",
            column: "NormalizedUserName")
            .Annotation("Npgsql:IndexMethod", "gin")
            .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
    }
}
