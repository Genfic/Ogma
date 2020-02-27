using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class BetterVoteStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Stories_StoryId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_StoryId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Votes");

            migrationBuilder.AddColumn<int>(
                name: "VotePoolId",
                table: "Votes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VotesPoolId",
                table: "Stories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VotePools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotePools", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VotePoolId",
                table: "Votes",
                column: "VotePoolId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Stories_VotesPoolId",
            //     table: "Stories",
            //     column: "VotesPoolId",
            //     unique: true);
            //
            // migrationBuilder.AddForeignKey(
            //     name: "FK_Stories_VotePools_VotesPoolId",
            //     table: "Stories",
            //     column: "VotesPoolId",
            //     principalTable: "VotePools",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

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
                name: "FK_Stories_VotePools_VotesPoolId",
                table: "Stories");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_VotePools_VotePoolId",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "VotePools");

            migrationBuilder.DropIndex(
                name: "IX_Votes_VotePoolId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Stories_VotesPoolId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "VotePoolId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "VotesPoolId",
                table: "Stories");

            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_StoryId",
                table: "Votes",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Stories_StoryId",
                table: "Votes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
