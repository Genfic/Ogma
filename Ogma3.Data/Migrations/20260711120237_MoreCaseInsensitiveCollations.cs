using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260711120237_MoreCaseInsensitiveCollations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Stories",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            collation: "nocase",
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Ratings",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            collation: "nocase-noaccent",
            oldClrType: typeof(string),
            oldType: "character varying(20)",
            oldMaxLength: 20);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Stories",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100,
            oldCollation: "nocase");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Ratings",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(20)",
            oldMaxLength: 20,
            oldCollation: "nocase-noaccent");
    }
}
