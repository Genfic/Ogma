using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class InviteCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InviteCode_AspNetUsers_UsedById",
                table: "InviteCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InviteCode",
                table: "InviteCode");

            migrationBuilder.RenameTable(
                name: "InviteCode",
                newName: "InviteCodes");

            migrationBuilder.RenameIndex(
                name: "IX_InviteCode_UsedById",
                table: "InviteCodes",
                newName: "IX_InviteCodes_UsedById");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCode",
                table: "InviteCodes",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InviteCodes",
                table: "InviteCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InviteCodes_AspNetUsers_UsedById",
                table: "InviteCodes",
                column: "UsedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InviteCodes_AspNetUsers_UsedById",
                table: "InviteCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InviteCodes",
                table: "InviteCodes");

            migrationBuilder.DropColumn(
                name: "NormalizedCode",
                table: "InviteCodes");

            migrationBuilder.RenameTable(
                name: "InviteCodes",
                newName: "InviteCode");

            migrationBuilder.RenameIndex(
                name: "IX_InviteCodes_UsedById",
                table: "InviteCode",
                newName: "IX_InviteCode_UsedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InviteCode",
                table: "InviteCode",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InviteCode_AspNetUsers_UsedById",
                table: "InviteCode",
                column: "UsedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
