using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class Followers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFollow",
                columns: table => new
                {
                    FollowingUserId = table.Column<long>(type: "bigint", nullable: false),
                    FollowedUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollow", x => new { x.FollowingUserId, x.FollowedUserId });
                    table.ForeignKey(
                        name: "FK_UserFollow_AspNetUsers_FollowedUserId",
                        column: x => x.FollowedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollow_AspNetUsers_FollowingUserId",
                        column: x => x.FollowingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollow_FollowedUserId",
                table: "UserFollow",
                column: "FollowedUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollow");
        }
    }
}
