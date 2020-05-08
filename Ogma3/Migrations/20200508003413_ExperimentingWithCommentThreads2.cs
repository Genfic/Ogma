using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ogma3.Migrations
{
    public partial class ExperimentingWithCommentThreads2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogposts_CommentThreads_CommentsThreadId",
                table: "Blogposts");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_CommentThreads_CommentsThreadId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_CommentsThreadId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CommentsThreadId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CommentsThreadId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "CommentsThreadId",
                table: "Blogposts");

            migrationBuilder.DropColumn(
                name: "CommentsThreadId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<long>(
                name: "BlogpostId",
                table: "CommentThreads",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChapterId",
                table: "CommentThreads",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "CommentThreads",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Chapters",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_CommentThreads_BlogpostId",
                table: "CommentThreads",
                column: "BlogpostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentThreads_ChapterId",
                table: "CommentThreads",
                column: "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentThreads_UserId",
                table: "CommentThreads",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentThreads_Blogposts_BlogpostId",
                table: "CommentThreads",
                column: "BlogpostId",
                principalTable: "Blogposts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentThreads_Chapters_ChapterId",
                table: "CommentThreads",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentThreads_AspNetUsers_UserId",
                table: "CommentThreads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentThreads_Blogposts_BlogpostId",
                table: "CommentThreads");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentThreads_Chapters_ChapterId",
                table: "CommentThreads");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentThreads_AspNetUsers_UserId",
                table: "CommentThreads");

            migrationBuilder.DropIndex(
                name: "IX_CommentThreads_BlogpostId",
                table: "CommentThreads");

            migrationBuilder.DropIndex(
                name: "IX_CommentThreads_ChapterId",
                table: "CommentThreads");

            migrationBuilder.DropIndex(
                name: "IX_CommentThreads_UserId",
                table: "CommentThreads");

            migrationBuilder.DropColumn(
                name: "BlogpostId",
                table: "CommentThreads");

            migrationBuilder.DropColumn(
                name: "ChapterId",
                table: "CommentThreads");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CommentThreads");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Chapters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "CommentsThreadId",
                table: "Chapters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CommentsThreadId",
                table: "Blogposts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CommentsThreadId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CommentsThreadId",
                table: "Chapters",
                column: "CommentsThreadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_CommentsThreadId",
                table: "Blogposts",
                column: "CommentsThreadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CommentsThreadId",
                table: "AspNetUsers",
                column: "CommentsThreadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommentThreads_CommentsThreadId",
                table: "AspNetUsers",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogposts_CommentThreads_CommentsThreadId",
                table: "Blogposts",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_CommentThreads_CommentsThreadId",
                table: "Chapters",
                column: "CommentsThreadId",
                principalTable: "CommentThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
