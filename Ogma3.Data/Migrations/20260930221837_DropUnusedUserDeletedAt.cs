using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260930221837_DropUnusedUserDeletedAt : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DeletedAt",
            table: "AspNetUsers");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DeletedAt",
            table: "AspNetUsers",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            column: "DeletedAt",
            value: null);

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            column: "DeletedAt",
            value: null);
    }
}
