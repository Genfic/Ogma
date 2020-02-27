using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class BetterVoteStructure5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateIndex(
                name: "IX_Stories_VotesPoolId",
                table: "Stories",
                column: "VotesPoolId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_VotePools_VotesPoolId",
                table: "Stories",
                column: "VotesPoolId",
                principalTable: "VotePools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_VotePools_VotesPoolId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_VotesPoolId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "VotesPoolId",
                table: "Stories");
        }
    }
}
