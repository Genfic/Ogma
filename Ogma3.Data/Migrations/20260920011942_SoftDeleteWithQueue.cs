using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260920011942_SoftDeleteWithQueue : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ScheduledForDeletion",
            table: "Stories",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ScheduledForDeletion",
            table: "Chapters",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ScheduledForDeletion",
            table: "Blogposts",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "SourceId",
            table: "CommentThreads",
            type: "bigint",
            nullable: false,
            computedColumnSql: "CASE\r\n	WHEN \"ChapterId\" IS NOT NULL THEN \"ChapterId\"\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN \"BlogpostId\"\r\n	WHEN \"UserId\" IS NOT NULL THEN \"UserId\"\r\n	WHEN \"ClubThreadId\" IS NOT NULL THEN \"ClubThreadId\"\r\n	WHEN \"NewsId\" IS NOT NULL THEN \"NewsId\"\r\nEND",
            stored: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "SourceId",
            table: "CommentThreads");

        migrationBuilder.DropColumn(
            name: "ScheduledForDeletion",
            table: "Stories");

        migrationBuilder.DropColumn(
            name: "ScheduledForDeletion",
            table: "Chapters");

        migrationBuilder.DropColumn(
            name: "ScheduledForDeletion",
            table: "Blogposts");
    }
}
