using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260724053541_BetterBlogpostExcerptsAndUserSafetyPin : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Reports_AspNetUsers_UserId",
            table: "Reports");

        migrationBuilder.AddColumn<int>(
            name: "ExcerptCutoff",
            table: "Blogposts",
            type: "integer",
            nullable: false,
            defaultValue: 200);

        migrationBuilder.AddColumn<string>(
            name: "SafetyPinHash",
            table: "AspNetUsers",
            type: "text",
            nullable: true);

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "ConcurrencyStamp", "Links", "SafetyPinHash" },
            values: new object[] { "acac5dc9-c9fa-4638-8bb2-7bf1b92a1217", new List<string>(), null });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "ConcurrencyStamp", "Links", "SafetyPinHash" },
            values: new object[] { "e660163e-aae1-4333-8692-e6415eff6625", new List<string>(), null });

        migrationBuilder.AddForeignKey(
            name: "FK_Reports_AspNetUsers_UserId",
            table: "Reports",
            column: "UserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Reports_AspNetUsers_UserId",
            table: "Reports");

        migrationBuilder.DropColumn(
            name: "ExcerptCutoff",
            table: "Blogposts");

        migrationBuilder.DropColumn(
            name: "SafetyPinHash",
            table: "AspNetUsers");

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "cb340b49-c5f6-4a37-b67e-fb3d7e826cc0", new List<string>() });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "3ab7e3c9-6e4f-4a33-8254-a7da234bf783", new List<string>() });

        migrationBuilder.AddForeignKey(
            name: "FK_Reports_AspNetUsers_UserId",
            table: "Reports",
            column: "UserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
