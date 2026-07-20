using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260720215302_BetterDocsAndTagLastChangeDate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Tags_Name",
            table: "Tags");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "LastChange",
            table: "Tags",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CustomCss",
            table: "Documents",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CustomJs",
            table: "Documents",
            type: "text",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tags_LastChange",
            table: "Tags",
            column: "LastChange",
            filter: "\"LastChange\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name",
            table: "Tags",
            column: "Name");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Tags_LastChange",
            table: "Tags");

        migrationBuilder.DropIndex(
            name: "IX_Tags_Name",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "LastChange",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "CustomCss",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "CustomJs",
            table: "Documents");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name",
            table: "Tags",
            column: "Name",
            unique: true);
    }
}
