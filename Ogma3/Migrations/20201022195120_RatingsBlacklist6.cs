using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class RatingsBlacklist6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlacklistedRatings",
                table: "BlacklistedRatings");

            migrationBuilder.DropIndex(
                name: "IX_BlacklistedRatings_UserId",
                table: "BlacklistedRatings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlacklistedRatings",
                table: "BlacklistedRatings",
                columns: new[] { "UserId", "RatingId" });

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedRatings_RatingId",
                table: "BlacklistedRatings",
                column: "RatingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlacklistedRatings",
                table: "BlacklistedRatings");

            migrationBuilder.DropIndex(
                name: "IX_BlacklistedRatings_RatingId",
                table: "BlacklistedRatings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlacklistedRatings",
                table: "BlacklistedRatings",
                columns: new[] { "RatingId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedRatings_UserId",
                table: "BlacklistedRatings",
                column: "UserId");
        }
    }
}
