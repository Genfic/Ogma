using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class VotesSystemInversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "BlogpostId",
                table: "VotePools",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StoryId",
                table: "VotePools",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotePools_Blogposts_BlogpostId",
                table: "VotePools");

            migrationBuilder.DropForeignKey(
                name: "FK_VotePools_Stories_StoryId",
                table: "VotePools");

            migrationBuilder.DropIndex(
                name: "IX_VotePools_BlogpostId",
                table: "VotePools");

            migrationBuilder.DropIndex(
                name: "IX_VotePools_StoryId",
                table: "VotePools");

            migrationBuilder.DropColumn(
                name: "BlogpostId",
                table: "VotePools");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "VotePools");

            migrationBuilder.AddColumn<long>(
                name: "VotesPoolId",
                table: "Stories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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
    }
}
