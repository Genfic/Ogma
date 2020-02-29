using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class UniqueVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_VotePoolId",
                table: "Votes",
                columns: new[] { "UserId", "VotePoolId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_VotePoolId",
                table: "Votes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                columns: new[] { "UserId", "VotePoolId" });
        }
    }
}
