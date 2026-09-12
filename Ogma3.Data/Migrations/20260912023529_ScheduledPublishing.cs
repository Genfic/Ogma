using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260912023529_ScheduledPublishing : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ScheduledFor",
            table: "Stories",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ScheduledFor",
            table: "Chapters",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ScheduledFor",
            table: "Blogposts",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Stories_ScheduledFor",
            table: "Stories",
            column: "ScheduledFor",
            filter: "\"ScheduledFor\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_ScheduledFor",
            table: "Chapters",
            column: "ScheduledFor",
            filter: "\"ScheduledFor\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_ScheduledFor",
            table: "Blogposts",
            column: "ScheduledFor",
            filter: "\"ScheduledFor\" IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Stories_ScheduledFor",
            table: "Stories");

        migrationBuilder.DropIndex(
            name: "IX_Chapters_ScheduledFor",
            table: "Chapters");

        migrationBuilder.DropIndex(
            name: "IX_Blogposts_ScheduledFor",
            table: "Blogposts");

        migrationBuilder.DropColumn(
            name: "ScheduledFor",
            table: "Stories");

        migrationBuilder.DropColumn(
            name: "ScheduledFor",
            table: "Chapters");

        migrationBuilder.DropColumn(
            name: "ScheduledFor",
            table: "Blogposts");
    }
}
