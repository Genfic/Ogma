using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class UserProfileCommentsCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentThreads_AspNetUsers_UserId",
                table: "CommentThreads");

            migrationBuilder.DropColumn(
                name: "CommentsThreadId",
                table: "Blogposts");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CommentThreads_AspNetUsers_UserId",
                table: "CommentThreads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentThreads_AspNetUsers_UserId",
                table: "CommentThreads");

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

            migrationBuilder.AddColumn<long>(
                name: "CommentsThreadId",
                table: "Blogposts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentThreads_AspNetUsers_UserId",
                table: "CommentThreads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
