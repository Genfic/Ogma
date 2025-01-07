using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class SomethingChangedBeforeAspireTrialsApparently : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_BlockedUserId",
                table: "BlacklistedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_BlockingUserId",
                table: "BlacklistedUsers");

            migrationBuilder.DropTable(
                name: "CommentsThreadSubscribers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlacklistedUsers",
                table: "BlacklistedUsers");

            migrationBuilder.RenameTable(
                name: "BlacklistedUsers",
                newName: "BlockedUsers");

            migrationBuilder.RenameIndex(
                name: "IX_BlacklistedUsers_BlockedUserId",
                table: "BlockedUsers",
                newName: "IX_BlockedUsers_BlockedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockedUsers",
                table: "BlockedUsers",
                columns: new[] { "BlockingUserId", "BlockedUserId" });

            migrationBuilder.CreateTable(
                name: "CommentThreadSubscribers",
                columns: table => new
                {
                    CommentsThreadId = table.Column<long>(type: "bigint", nullable: false),
                    OgmaUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentThreadSubscribers", x => new { x.CommentsThreadId, x.OgmaUserId });
                    table.ForeignKey(
                        name: "FK_CommentThreadSubscribers_AspNetUsers_OgmaUserId",
                        column: x => x.OgmaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentThreadSubscribers_CommentThreads_CommentsThreadId",
                        column: x => x.CommentsThreadId,
                        principalTable: "CommentThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentThreadSubscribers_OgmaUserId",
                table: "CommentThreadSubscribers",
                column: "OgmaUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockedUserId",
                table: "BlockedUsers",
                column: "BlockedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockingUserId",
                table: "BlockedUsers",
                column: "BlockingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockedUserId",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockingUserId",
                table: "BlockedUsers");

            migrationBuilder.DropTable(
                name: "CommentThreadSubscribers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockedUsers",
                table: "BlockedUsers");

            migrationBuilder.RenameTable(
                name: "BlockedUsers",
                newName: "BlacklistedUsers");

            migrationBuilder.RenameIndex(
                name: "IX_BlockedUsers_BlockedUserId",
                table: "BlacklistedUsers",
                newName: "IX_BlacklistedUsers_BlockedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlacklistedUsers",
                table: "BlacklistedUsers",
                columns: new[] { "BlockingUserId", "BlockedUserId" });

            migrationBuilder.CreateTable(
                name: "CommentsThreadSubscribers",
                columns: table => new
                {
                    CommentsThreadId = table.Column<long>(type: "bigint", nullable: false),
                    OgmaUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsThreadSubscribers", x => new { x.CommentsThreadId, x.OgmaUserId });
                    table.ForeignKey(
                        name: "FK_CommentsThreadSubscribers_AspNetUsers_OgmaUserId",
                        column: x => x.OgmaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsThreadSubscribers_CommentThreads_CommentsThreadId",
                        column: x => x.CommentsThreadId,
                        principalTable: "CommentThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentsThreadSubscribers_OgmaUserId",
                table: "CommentsThreadSubscribers",
                column: "OgmaUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_BlockedUserId",
                table: "BlacklistedUsers",
                column: "BlockedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlacklistedUsers_AspNetUsers_BlockingUserId",
                table: "BlacklistedUsers",
                column: "BlockingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
