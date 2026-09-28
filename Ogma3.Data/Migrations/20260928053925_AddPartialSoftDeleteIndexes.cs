using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260928053925_AddPartialSoftDeleteIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Stories_ScheduledForDeletion",
            table: "Stories",
            column: "ScheduledForDeletion",
            filter: "\"ScheduledForDeletion\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_ScheduledForDeletion",
            table: "Chapters",
            column: "ScheduledForDeletion",
            filter: "\"ScheduledForDeletion\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_ScheduledForDeletion",
            table: "Blogposts",
            column: "ScheduledForDeletion",
            filter: "\"ScheduledForDeletion\" IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Stories_ScheduledForDeletion",
            table: "Stories");

        migrationBuilder.DropIndex(
            name: "IX_Chapters_ScheduledForDeletion",
            table: "Chapters");

        migrationBuilder.DropIndex(
            name: "IX_Blogposts_ScheduledForDeletion",
            table: "Blogposts");
    }
}
