using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260624012747_InviteCodesNowHaveOptionalIssuer : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<long>(
            name: "IssuedById",
            table: "InviteCodes",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AddColumn<string>(
            name: "IssuedByType",
            table: "InviteCodes",
            type: "character varying(32)",
            maxLength: 32,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IssuedByType",
            table: "InviteCodes");

        migrationBuilder.AlterColumn<long>(
            name: "IssuedById",
            table: "InviteCodes",
            type: "bigint",
            nullable: false,
            defaultValue: 0L,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);
    }
}
