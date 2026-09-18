using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260918102112_PossiblyBetterPrimitiveCollectionsConfig : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Blogposts_Hashtags",
            table: "Blogposts");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_Hashtags",
            table: "Blogposts",
            column: "Hashtags",
            filter: "\"IsVisible\"")
            .Annotation("Npgsql:IndexMethod", "gin");

        migrationBuilder.AddCheckConstraint(
            name: "CK_Blogpost_Hashtags_cardinality",
            table: "Blogposts",
            sql: "cardinality(\"Hashtags\") <= 20");

        migrationBuilder.AddCheckConstraint(
            name: "CK_OgmaUser_Links_cardinality",
            table: "AspNetUsers",
            sql: "cardinality(\"Links\") <= 5");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Blogposts_Hashtags",
            table: "Blogposts");

        migrationBuilder.DropCheckConstraint(
            name: "CK_Blogpost_Hashtags_cardinality",
            table: "Blogposts");

        migrationBuilder.DropCheckConstraint(
            name: "CK_OgmaUser_Links_cardinality",
            table: "AspNetUsers");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_Hashtags",
            table: "Blogposts",
            column: "Hashtags",
            filter: "\"IsVisible\"")
            .Annotation("Npgsql:IndexMethod", "gin")
            .Annotation("Relational:Collation", new[] { "nocase-noaccent" });
    }
}
