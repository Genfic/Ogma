using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class InviteCodesStoredWithCaseInsensitivityNoNeedForNormalizedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedCode",
                table: "InviteCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "InviteCodes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                collation: "nocase-noaccent",
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "InviteCodes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldCollation: "nocase-noaccent");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCode",
                table: "InviteCodes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
