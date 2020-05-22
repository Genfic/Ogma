using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ogma3.Migrations
{
    public partial class Clubs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Hook = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    IconId = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubMembers",
                columns: table => new
                {
                    MemberId = table.Column<long>(nullable: false),
                    ClubId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMembers", x => new { x.ClubId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_ClubMembers_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClubMembers_AspNetUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubThreads",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(maxLength: 20, nullable: false),
                    Body = table.Column<string>(maxLength: 10000, nullable: false),
                    AuthorId = table.Column<long>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ClubId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClubThreads_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClubThreads_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubThreadComments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Body = table.Column<string>(nullable: true),
                    AuthorId = table.Column<long>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    ClubThreadId = table.Column<long>(nullable: true)
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
                name: "IX_ClubMembers_MemberId",
                table: "ClubMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubThreadComments_AuthorId",
                table: "ClubThreadComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubThreadComments_ClubThreadId",
                table: "ClubThreadComments",
                column: "ClubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubThreads_AuthorId",
                table: "ClubThreads",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubThreads_ClubId",
                table: "ClubThreads",
                column: "ClubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubMembers");

            migrationBuilder.DropTable(
                name: "ClubThreadComments");

            migrationBuilder.DropTable(
                name: "ClubThreads");

            migrationBuilder.DropTable(
                name: "Clubs");
        }
    }
}
