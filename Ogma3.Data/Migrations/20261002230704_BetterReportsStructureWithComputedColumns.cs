using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20261002230704_BetterReportsStructureWithComputedColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<short>(
            name: "ContentType",
            table: "Reports",
            type: "smallint",
            nullable: false,
            computedColumnSql: "CASE\r\n	WHEN \"ChapterId\"  IS NOT NULL THEN 3\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN 4\r\n   WHEN \"StoryId\"    IS NOT NULL THEN 2\r\n	WHEN \"UserId\"     IS NOT NULL THEN 1\r\n	WHEN \"CommentId\"  IS NOT NULL THEN 0\r\n	WHEN \"ClubId\"     IS NOT NULL THEN 5\r\nEND",
            stored: true,
            oldClrType: typeof(string),
            oldType: "character varying(32)",
            oldMaxLength: 32);

        migrationBuilder.AddColumn<long>(
            name: "ContentId",
            table: "Reports",
            type: "bigint",
            nullable: false,
            computedColumnSql: "CASE\r\n	WHEN \"ChapterId\"  IS NOT NULL THEN \"ChapterId\"\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN \"BlogpostId\"\r\n   WHEN \"StoryId\"    IS NOT NULL THEN \"StoryId\"\r\n	WHEN \"UserId\"     IS NOT NULL THEN \"UserId\"\r\n	WHEN \"CommentId\"  IS NOT NULL THEN \"CommentId\"\r\n	WHEN \"ClubId\"     IS NOT NULL THEN \"ClubId\"\r\nEND",
            stored: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ContentId",
            table: "Reports");

        migrationBuilder.AlterColumn<string>(
            name: "ContentType",
            table: "Reports",
            type: "character varying(32)",
            maxLength: 32,
            nullable: false,
            oldClrType: typeof(short),
            oldType: "smallint",
            oldComputedColumnSql: "CASE\r\n	WHEN \"ChapterId\"  IS NOT NULL THEN 3\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN 4\r\n   WHEN \"StoryId\"    IS NOT NULL THEN 2\r\n	WHEN \"UserId\"     IS NOT NULL THEN 1\r\n	WHEN \"CommentId\"  IS NOT NULL THEN 0\r\n	WHEN \"ClubId\"     IS NOT NULL THEN 5\r\nEND");
    }
}
