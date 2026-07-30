using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260730035803_CompleteSafetyPinSetup : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_GlobalNotification_AspNetUsers_CreatedById",
            table: "GlobalNotification");

        migrationBuilder.DropPrimaryKey(
            name: "PK_GlobalNotification",
            table: "GlobalNotification");

        migrationBuilder.RenameTable(
            name: "GlobalNotification",
            newName: "GlobalNotifications");

        migrationBuilder.RenameIndex(
            name: "IX_GlobalNotification_CreatedById",
            table: "GlobalNotifications",
            newName: "IX_GlobalNotifications_CreatedById");

        migrationBuilder.AlterColumn<string>(
            name: "SafetyPinHash",
            table: "AspNetUsers",
            type: "character varying(512)",
            maxLength: 512,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "SafetyPinLockedOutUntil",
            table: "AspNetUsers",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "SafetyPinResetTokenExpiry",
            table: "AspNetUsers",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "SafetyPinResetTokenHash",
            table: "AspNetUsers",
            type: "character varying(512)",
            maxLength: 512,
            nullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_GlobalNotifications",
            table: "GlobalNotifications",
            column: "Id");

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "Links", "SafetyPinLockedOutUntil", "SafetyPinResetTokenExpiry", "SafetyPinResetTokenHash" },
            values: new object[] { new List<string>(), null, null, null });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "Links", "SafetyPinLockedOutUntil", "SafetyPinResetTokenExpiry", "SafetyPinResetTokenHash" },
            values: new object[] { new List<string>(), null, null, null });

        migrationBuilder.AddForeignKey(
            name: "FK_GlobalNotifications_AspNetUsers_CreatedById",
            table: "GlobalNotifications",
            column: "CreatedById",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_GlobalNotifications_AspNetUsers_CreatedById",
            table: "GlobalNotifications");

        migrationBuilder.DropPrimaryKey(
            name: "PK_GlobalNotifications",
            table: "GlobalNotifications");

        migrationBuilder.DropColumn(
            name: "SafetyPinLockedOutUntil",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "SafetyPinResetTokenExpiry",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "SafetyPinResetTokenHash",
            table: "AspNetUsers");

        migrationBuilder.RenameTable(
            name: "GlobalNotifications",
            newName: "GlobalNotification");

        migrationBuilder.RenameIndex(
            name: "IX_GlobalNotifications_CreatedById",
            table: "GlobalNotification",
            newName: "IX_GlobalNotification_CreatedById");

        migrationBuilder.AlterColumn<string>(
            name: "SafetyPinHash",
            table: "AspNetUsers",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(512)",
            oldMaxLength: 512,
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_GlobalNotification",
            table: "GlobalNotification",
            column: "Id");

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            column: "Links",
            value: new List<string>());

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            column: "Links",
            value: new List<string>());

        migrationBuilder.AddForeignKey(
            name: "FK_GlobalNotification_AspNetUsers_CreatedById",
            table: "GlobalNotification",
            column: "CreatedById",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
