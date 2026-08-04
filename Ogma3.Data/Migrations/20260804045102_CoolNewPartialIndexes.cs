using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260804045102_CoolNewPartialIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_BlogpostId",
            table: "CommentThreads");

        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_ChapterId",
            table: "CommentThreads");

        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_ClubThreadId",
            table: "CommentThreads");

        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_UserId",
            table: "CommentThreads");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_BlogpostId",
            table: "CommentThreads",
            column: "BlogpostId",
            unique: true,
            filter: "\"BlogpostId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_ChapterId",
            table: "CommentThreads",
            column: "ChapterId",
            unique: true,
            filter: "\"ChapterId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_ClubThreadId",
            table: "CommentThreads",
            column: "ClubThreadId",
            unique: true,
            filter: "\"ClubThreadId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_UserId",
            table: "CommentThreads",
            column: "UserId",
            unique: true,
            filter: "\"UserId\" IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_BlogpostId",
            table: "CommentThreads");

        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_ChapterId",
            table: "CommentThreads");

        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_ClubThreadId",
            table: "CommentThreads");

        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_UserId",
            table: "CommentThreads");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_BlogpostId",
            table: "CommentThreads",
            column: "BlogpostId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_ChapterId",
            table: "CommentThreads",
            column: "ChapterId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_ClubThreadId",
            table: "CommentThreads",
            column: "ClubThreadId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_UserId",
            table: "CommentThreads",
            column: "UserId",
            unique: true);
    }
}
