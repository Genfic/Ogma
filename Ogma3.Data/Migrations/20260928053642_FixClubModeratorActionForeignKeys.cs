using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260928053642_FixClubModeratorActionForeignKeys : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ClubModeratorActions_Clubs_ModeratorId",
            table: "ClubModeratorActions");

        migrationBuilder.AlterColumn<long>(
            name: "ModeratorId",
            table: "ClubModeratorActions",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.CreateIndex(
            name: "IX_ClubModeratorActions_ClubId",
            table: "ClubModeratorActions",
            column: "ClubId");

        migrationBuilder.AddForeignKey(
            name: "FK_ClubModeratorActions_Clubs_ClubId",
            table: "ClubModeratorActions",
            column: "ClubId",
            principalTable: "Clubs",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ClubModeratorActions_Clubs_ClubId",
            table: "ClubModeratorActions");

        migrationBuilder.DropIndex(
            name: "IX_ClubModeratorActions_ClubId",
            table: "ClubModeratorActions");

        migrationBuilder.AlterColumn<long>(
            name: "ModeratorId",
            table: "ClubModeratorActions",
            type: "bigint",
            nullable: false,
            defaultValue: 0L,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_ClubModeratorActions_Clubs_ModeratorId",
            table: "ClubModeratorActions",
            column: "ModeratorId",
            principalTable: "Clubs",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
