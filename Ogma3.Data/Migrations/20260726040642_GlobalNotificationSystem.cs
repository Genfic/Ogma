using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260726040642_GlobalNotificationSystem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GlobalNotification",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Message = table.Column<string>(type: "text", maxLength: -1, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Color = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                CreatedById = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GlobalNotification", x => x.Id);
                table.ForeignKey(
                    name: "FK_GlobalNotification_AspNetUsers_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

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

        migrationBuilder.CreateIndex(
            name: "IX_GlobalNotification_CreatedById",
            table: "GlobalNotification",
            column: "CreatedById");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GlobalNotification");

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "acac5dc9-c9fa-4638-8bb2-7bf1b92a1217", new List<string>() });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "ConcurrencyStamp", "Links" },
            values: new object[] { "e660163e-aae1-4333-8692-e6415eff6625", new List<string>() });
    }
}
