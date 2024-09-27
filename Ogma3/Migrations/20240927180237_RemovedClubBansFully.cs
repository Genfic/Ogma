using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class RemovedClubBansFully : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubOgmaUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClubOgmaUser",
                columns: table => new
                {
                    BannedUsersId = table.Column<long>(type: "bigint", nullable: false),
                    ClubsBannedFromId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubOgmaUser", x => new { x.BannedUsersId, x.ClubsBannedFromId });
                    table.ForeignKey(
                        name: "FK_ClubOgmaUser_AspNetUsers_BannedUsersId",
                        column: x => x.BannedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubOgmaUser_Clubs_ClubsBannedFromId",
                        column: x => x.ClubsBannedFromId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubOgmaUser_ClubsBannedFromId",
                table: "ClubOgmaUser",
                column: "ClubsBannedFromId");
        }
    }
}
