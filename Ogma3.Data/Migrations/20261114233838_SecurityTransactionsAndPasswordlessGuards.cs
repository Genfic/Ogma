using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20261114233838_SecurityTransactionsAndPasswordlessGuards : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "PasswordResetTokenExpiry",
            table: "AspNetUsers",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PasswordResetTokenHash",
            table: "AspNetUsers",
            type: "text",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "SecurityTransactions",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                SessionId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Operation = table.Column<int>(type: "integer", nullable: false),
                Target = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                SecurityStamp = table.Column<string>(type: "text", nullable: false),
                ProposedSecret = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                AllowedProofs = table.Column<int>(type: "integer", nullable: false),
                CompletedProof = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                VerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ConsumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SecurityTransactions", x => x.Id);
            });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -2L,
            columns: new[] { "PasswordResetTokenExpiry", "PasswordResetTokenHash" },
            values: new object[] { null, null });

        migrationBuilder.UpdateData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: -1L,
            columns: new[] { "PasswordResetTokenExpiry", "PasswordResetTokenHash" },
            values: new object[] { null, null });

        migrationBuilder.CreateIndex(
            name: "IX_SecurityTransactions_UserId",
            table: "SecurityTransactions",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityTransactions_UserId_SessionId",
            table: "SecurityTransactions",
            columns: new[] { "UserId", "SessionId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SecurityTransactions");

        migrationBuilder.DropColumn(
            name: "PasswordResetTokenExpiry",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "PasswordResetTokenHash",
            table: "AspNetUsers");
    }
}
