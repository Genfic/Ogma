using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class InviteCodeIssuer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IssuedById",
                table: "InviteCodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_InviteCodes_IssuedById",
                table: "InviteCodes",
                column: "IssuedById");

            migrationBuilder.AddForeignKey(
                name: "FK_InviteCodes_AspNetUsers_IssuedById",
                table: "InviteCodes",
                column: "IssuedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InviteCodes_AspNetUsers_IssuedById",
                table: "InviteCodes");

            migrationBuilder.DropIndex(
                name: "IX_InviteCodes_IssuedById",
                table: "InviteCodes");

            migrationBuilder.DropColumn(
                name: "IssuedById",
                table: "InviteCodes");
        }
    }
}
