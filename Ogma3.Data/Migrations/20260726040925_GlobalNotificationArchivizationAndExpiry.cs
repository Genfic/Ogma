using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260726040925_GlobalNotificationArchivizationAndExpiry : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ArchivedAt",
            table: "GlobalNotification",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ExpiresAt",
            table: "GlobalNotification",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "a61c70e7-5201-4783-bb60-325f49f18e73", new List<string>() });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "1ce8427d-7e86-423a-ba9d-15914b09b6c8", new List<string>() });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ArchivedAt",
            table: "GlobalNotification");

        migrationBuilder.DropColumn(
            name: "ExpiresAt",
            table: "GlobalNotification");

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "29006130-47f7-4df3-a0b5-0d160cc6b63d", new List<string>() });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "3007e8a5-7b43-480b-b131-0abf2ee980d3", new List<string>() });
    }
}
