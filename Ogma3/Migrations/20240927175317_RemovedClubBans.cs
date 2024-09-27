using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class RemovedClubBans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubBans");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubOgmaUser");

            migrationBuilder.CreateTable(
                name: "ClubBans",
                columns: table => new
                {
                    ClubId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IssuerId = table.Column<long>(type: "bigint", nullable: false),
                    BanDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubBans", x => new { x.ClubId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ClubBans_AspNetUsers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubBans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubBans_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubBans_IssuerId",
                table: "ClubBans",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubBans_UserId",
                table: "ClubBans",
                column: "UserId");
        }
    }
}
