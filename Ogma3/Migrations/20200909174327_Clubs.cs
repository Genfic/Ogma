using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ogma3.Migrations
{
    public partial class Clubs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubThreadComments");

            migrationBuilder.AddColumn<long>(
                name: "ClubThreadId",
                table: "CommentThreads",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentThreads_ClubThreadId",
                table: "CommentThreads",
                column: "ClubThreadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentThreads_ClubThreads_ClubThreadId",
                table: "CommentThreads",
                column: "ClubThreadId",
                principalTable: "ClubThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentThreads_ClubThreads_ClubThreadId",
                table: "CommentThreads");

            migrationBuilder.DropIndex(
                name: "IX_CommentThreads_ClubThreadId",
                table: "CommentThreads");

            migrationBuilder.DropColumn(
                name: "ClubThreadId",
                table: "CommentThreads");

            migrationBuilder.CreateTable(
                name: "ClubThreadComments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Body = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    ClubThreadId = table.Column<long>(type: "bigint", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubThreadComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubThreadComments_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClubThreadComments_ClubThreads_ClubThreadId",
                        column: x => x.ClubThreadId,
                        principalTable: "ClubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubThreadComments_AuthorId",
                table: "ClubThreadComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubThreadComments_ClubThreadId",
                table: "ClubThreadComments",
                column: "ClubThreadId");
        }
    }
}
