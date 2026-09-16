using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260916192631_FixedBlogpostHashtagFilteringMaybe2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string[]>(
            name: "Hashtags",
            table: "Blogposts",
            type: "character varying(20)[]",
            maxLength: 10,
            nullable: false,
            defaultValue: new string[0],
            oldClrType: typeof(string[]),
            oldType: "text[]",
            oldDefaultValue: new string[0]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string[]>(
            name: "Hashtags",
            table: "Blogposts",
            type: "text[]",
            nullable: false,
            defaultValue: new string[0],
            oldClrType: typeof(string[]),
            oldType: "character varying(20)[]",
            oldMaxLength: 10,
            oldDefaultValue: new string[0]);
    }
}
