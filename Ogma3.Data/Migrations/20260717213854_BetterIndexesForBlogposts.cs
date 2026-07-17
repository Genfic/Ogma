using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260717213854_BetterIndexesForBlogposts : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Blogposts_CreationDate",
            table: "Blogposts");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Blogposts",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            collation: "nocase",
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100);

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_Hashtags",
            table: "Blogposts",
            column: "Hashtags",
            filter: "\"IsVisible\"")
            .Annotation("Npgsql:IndexMethod", "gin")
            .Annotation("Relational:Collation", new[] { "nocase-noaccent" });

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_Title",
            table: "Blogposts",
            column: "Title");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Blogposts_Hashtags",
            table: "Blogposts");

        migrationBuilder.DropIndex(
            name: "IX_Blogposts_Title",
            table: "Blogposts");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Blogposts",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100,
            oldCollation: "nocase");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_CreationDate",
            table: "Blogposts",
            column: "CreationDate");
    }
}
