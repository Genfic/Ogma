using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class InviteCodeIssuer2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InviteCodes_IssuedById",
                table: "InviteCodes");

            migrationBuilder.CreateIndex(
                name: "IX_InviteCodes_IssuedById",
                table: "InviteCodes",
                column: "IssuedById",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InviteCodes_IssuedById",
                table: "InviteCodes");

            migrationBuilder.CreateIndex(
                name: "IX_InviteCodes_IssuedById",
                table: "InviteCodes",
                column: "IssuedById");
        }
    }
}
