using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260726041344_BetterUserSeeding : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Message",
            table: "GlobalNotification",
            type: "character varying(512)",
            maxLength: 512,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text",
            oldMaxLength: -1);

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "ConcurrencyStamp", "Links", "SecurityStamp" },
            values: new object[] { "00000000-0000-0000-0000-000000000002", new List<string>(), "00000000-0000-0000-0000-000000000002" });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "ConcurrencyStamp", "Links", "SecurityStamp" },
            values: new object[] { "00000000-0000-0000-0000-000000000001", new List<string>(), "00000000-0000-0000-0000-000000000001" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Message",
            table: "GlobalNotification",
            type: "text",
            maxLength: -1,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(512)",
            oldMaxLength: 512);

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "ConcurrencyStamp", "Links", "SecurityStamp" },
            values: new object[] { "a61c70e7-5201-4783-bb60-325f49f18e73", new List<string>(), null });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "ConcurrencyStamp", "Links", "SecurityStamp" },
            values: new object[] { "1ce8427d-7e86-423a-ba9d-15914b09b6c8", new List<string>(), null });
    }
}
