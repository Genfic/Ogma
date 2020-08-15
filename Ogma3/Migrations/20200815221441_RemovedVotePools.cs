using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class RemovedVotePools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotePools_Blogposts_BlogpostId",
                table: "VotePools");

            migrationBuilder.DropForeignKey(
                name: "FK_VotePools_Stories_StoryId",
                table: "VotePools");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_VotePoolId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_VotePools_BlogpostId",
                table: "VotePools");

            migrationBuilder.DropIndex(
                name: "IX_VotePools_StoryId",
                table: "VotePools");

            migrationBuilder.AlterColumn<long>(
                name: "VotePoolId",
                table: "Votes",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "StoryId",
                table: "Votes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_StoryId",
                table: "Votes",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_StoryId",
                table: "Votes",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotePools_BlogpostId",
                table: "VotePools",
                column: "BlogpostId");

            migrationBuilder.CreateIndex(
                name: "IX_VotePools_StoryId",
                table: "VotePools",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VotePools_Blogposts_BlogpostId",
                table: "VotePools",
                column: "BlogpostId",
                principalTable: "Blogposts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VotePools_Stories_StoryId",
                table: "VotePools",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Stories_StoryId",
                table: "Votes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotePools_Blogposts_BlogpostId",
                table: "VotePools");

            migrationBuilder.DropForeignKey(
                name: "FK_VotePools_Stories_StoryId",
                table: "VotePools");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Stories_StoryId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_StoryId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_StoryId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_VotePools_BlogpostId",
                table: "VotePools");

            migrationBuilder.DropIndex(
                name: "IX_VotePools_StoryId",
                table: "VotePools");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Votes");

            migrationBuilder.AlterColumn<long>(
                name: "VotePoolId",
                table: "Votes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_VotePoolId",
                table: "Votes",
                columns: new[] { "UserId", "VotePoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotePools_BlogpostId",
                table: "VotePools",
                column: "BlogpostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotePools_StoryId",
                table: "VotePools",
                column: "StoryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VotePools_Blogposts_BlogpostId",
                table: "VotePools",
                column: "BlogpostId",
                principalTable: "Blogposts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VotePools_Stories_StoryId",
                table: "VotePools",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
