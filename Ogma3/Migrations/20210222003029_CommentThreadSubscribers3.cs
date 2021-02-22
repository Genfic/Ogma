using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class CommentThreadSubscribers3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsThreadSubscriber_AspNetUsers_OgmaUserId",
                table: "CommentsThreadSubscriber");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentsThreadSubscriber_CommentThreads_CommentsThreadId",
                table: "CommentsThreadSubscriber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentsThreadSubscriber",
                table: "CommentsThreadSubscriber");

            migrationBuilder.RenameTable(
                name: "CommentsThreadSubscriber",
                newName: "CommentsThreadSubscribers");

            migrationBuilder.RenameIndex(
                name: "IX_CommentsThreadSubscriber_OgmaUserId",
                table: "CommentsThreadSubscribers",
                newName: "IX_CommentsThreadSubscribers_OgmaUserId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentsThreadSubscribers",
                table: "CommentsThreadSubscribers",
                columns: new[] { "CommentsThreadId", "OgmaUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsThreadSubscribers_AspNetUsers_OgmaUserId",
                table: "CommentsThreadSubscribers",
                column: "OgmaUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsThreadSubscribers_CommentThreads_CommentsThreadId",
                table: "CommentsThreadSubscribers",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsThreadSubscribers_AspNetUsers_OgmaUserId",
                table: "CommentsThreadSubscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentsThreadSubscribers_CommentThreads_CommentsThreadId",
                table: "CommentsThreadSubscribers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentsThreadSubscribers",
                table: "CommentsThreadSubscribers");

            migrationBuilder.RenameTable(
                name: "CommentsThreadSubscribers",
                newName: "CommentsThreadSubscriber");

            migrationBuilder.RenameIndex(
                name: "IX_CommentsThreadSubscribers_OgmaUserId",
                table: "CommentsThreadSubscriber",
                newName: "IX_CommentsThreadSubscriber_OgmaUserId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentsThreadSubscriber",
                table: "CommentsThreadSubscriber",
                columns: new[] { "CommentsThreadId", "OgmaUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsThreadSubscriber_AspNetUsers_OgmaUserId",
                table: "CommentsThreadSubscriber",
                column: "OgmaUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsThreadSubscriber_CommentThreads_CommentsThreadId",
                table: "CommentsThreadSubscriber",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
