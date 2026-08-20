using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260820010802_BetterWayOfGettingCommentThreadSource : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<short>(
            name: "Source",
            table: "CommentThreads",
            type: "smallint",
            nullable: false,
            computedColumnSql: "CASE\r\n	WHEN \"ChapterId\" IS NOT NULL THEN 0\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN 1\r\n	WHEN \"UserId\" IS NOT NULL THEN 2\r\n	WHEN \"ClubThreadId\" IS NOT NULL THEN 3\r\n	WHEN \"NewsId\" IS NOT NULL THEN 4\r\nEND",
            stored: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Source",
            table: "CommentThreads");
    }
}
