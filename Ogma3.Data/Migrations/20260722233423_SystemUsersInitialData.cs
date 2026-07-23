using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260722233423_SystemUsersInitialData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "TimeZone",
            table: "AspNetUsers",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "UTC",
            oldClrType: typeof(string),
            oldType: "character varying(50)",
            oldMaxLength: 50);

        migrationBuilder.InsertData(
            table: "Images",
            columns: new[] { "Id", "ETag", "Url" },
            values: new object[,]
            {
                { -2L, null, "/img/placeholders/anonymous-user.png" },
                { -1L, null, "/img/placeholders/deleted-user.png" }
            });

        migrationBuilder.InsertData(
            table: "AspNetUsers",
            columns: new[] { "Id", "AccessFailedCount", "AvatarId", "Bio", "ConcurrencyStamp", "DeletedAt", "Email", "EmailConfirmed", "Links", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "SubscriptionId", "TimeZone", "Title", "TwoFactorEnabled", "UserName" },
            values: new object[,]
            {
                { -2L, 0, -2L, null, "cb340b49-c5f6-4a37-b67e-fb3d7e826cc0", null, "", false, new List<string>(), false, null, "", "ANONYMOUS USER", null, null, null, "UTC", null, false, "Anonymous User" },
                { -1L, 0, -1L, null, "3ab7e3c9-6e4f-4a33-8254-a7da234bf783", null, "", false, new List<string>(), false, null, "", "DELETED USER", null, null, null, "UTC", null, false, "Deleted User" }
            });

        migrationBuilder.InsertData(
            table: "CommentThreads",
            columns: new[] { "Id", "BlogpostId", "ChapterId", "ClubThreadId", "LockDate", "UserId" },
            values: new object[,]
            {
                { -2L, null, null, null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), -2L },
                { -1L, null, null, null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), -1L }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CommentThreads",
            keyColumn: "Id",
            keyValue: -2L);

        migrationBuilder.DeleteData(
            table: "CommentThreads",
            keyColumn: "Id",
            keyValue: -1L);

        migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L);

        migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L);

        migrationBuilder.DeleteData(
            table: "Images",
            keyColumn: "Id",
            keyValue: -2L);

        migrationBuilder.DeleteData(
            table: "Images",
            keyColumn: "Id",
            keyValue: -1L);

        migrationBuilder.AlterColumn<string>(
            name: "TimeZone",
            table: "AspNetUsers",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(50)",
            oldMaxLength: 50,
            oldDefaultValue: "UTC");
    }
}
