using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class MoreClubFunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FounderId",
                table: "Clubs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClubStories",
                columns: table => new
                {
                    ClubId = table.Column<long>(nullable: false),
                    StoryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubStories", x => new { x.ClubId, x.StoryId });
                    table.ForeignKey(
                        name: "FK_ClubStories_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubStories_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_FounderId",
                table: "Clubs",
                column: "FounderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubStories_StoryId",
                table: "ClubStories",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_AspNetUsers_FounderId",
                table: "Clubs",
                column: "FounderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_AspNetUsers_FounderId",
                table: "Clubs");

            migrationBuilder.DropTable(
                name: "ClubStories");

            migrationBuilder.DropIndex(
                name: "IX_Clubs_FounderId",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "FounderId",
                table: "Clubs");
        }
    }
}
