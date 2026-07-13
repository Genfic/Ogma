using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260712231045_NonCanonicalStoryTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "Tags",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<long>(
            name: "CreatedById",
            table: "Tags",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<List<string>>(
            name: "ExtraTags",
            table: "Stories",
            type: "text[]",
            nullable: false,
            defaultValueSql: "'{}'");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_CreatedById",
            table: "Tags",
            column: "CreatedById");

        migrationBuilder.AddForeignKey(
            name: "FK_Tags_AspNetUsers_CreatedById",
            table: "Tags",
            column: "CreatedById",
            principalTable: "AspNetUsers",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Tags_AspNetUsers_CreatedById",
            table: "Tags");

        migrationBuilder.DropIndex(
            name: "IX_Tags_CreatedById",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "CreatedById",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "ExtraTags",
            table: "Stories");
    }
}
