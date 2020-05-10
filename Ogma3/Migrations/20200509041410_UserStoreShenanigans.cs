using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class UserStoreShenanigans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_VotePools_VotePoolId1",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_VotePoolId1",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "VotePoolId1",
                table: "Votes");

            migrationBuilder.AlterColumn<long>(
                name: "VotePoolId",
                table: "Votes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VotePoolId",
                table: "Votes",
                column: "VotePoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_VotePools_VotePoolId",
                table: "Votes",
                column: "VotePoolId",
                principalTable: "VotePools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_VotePools_VotePoolId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_VotePoolId",
                table: "Votes");

            migrationBuilder.AlterColumn<int>(
                name: "VotePoolId",
                table: "Votes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "VotePoolId1",
                table: "Votes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VotePoolId1",
                table: "Votes",
                column: "VotePoolId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_VotePools_VotePoolId1",
                table: "Votes",
                column: "VotePoolId1",
                principalTable: "VotePools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
