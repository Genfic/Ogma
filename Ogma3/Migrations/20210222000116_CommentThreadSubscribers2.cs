using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class CommentThreadSubscribers2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentsThreadOgmaUser");

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "CommentsThreadSubscriber",
                columns: table => new
                {
                    CommentsThreadId = table.Column<long>(type: "bigint", nullable: false),
                    OgmaUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsThreadSubscriber", x => new { x.CommentsThreadId, x.OgmaUserId });
                    table.ForeignKey(
                        name: "FK_CommentsThreadSubscriber_AspNetUsers_OgmaUserId",
                        column: x => x.OgmaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsThreadSubscriber_CommentThreads_CommentsThreadId",
                        column: x => x.CommentsThreadId,
                        principalTable: "CommentThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentsThreadSubscriber_OgmaUserId",
                table: "CommentsThreadSubscriber",
                column: "OgmaUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentsThreadSubscriber");

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "CommentsThreadOgmaUser",
                columns: table => new
                {
                    SubcribersId = table.Column<long>(type: "bigint", nullable: false),
                    SubscribedThreadsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsThreadOgmaUser", x => new { x.SubcribersId, x.SubscribedThreadsId });
                    table.ForeignKey(
                        name: "FK_CommentsThreadOgmaUser_AspNetUsers_SubcribersId",
                        column: x => x.SubcribersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsThreadOgmaUser_CommentThreads_SubscribedThreadsId",
                        column: x => x.SubscribedThreadsId,
                        principalTable: "CommentThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentsThreadOgmaUser_SubscribedThreadsId",
                table: "CommentsThreadOgmaUser",
                column: "SubscribedThreadsId");
        }
    }
}
