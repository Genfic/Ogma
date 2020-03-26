using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ogma3.Migrations
{
    public partial class AddedBlogposts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blogposts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Slug = table.Column<string>(nullable: false),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    AuthorId = table.Column<string>(nullable: false),
                    Body = table.Column<string>(maxLength: 500000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogposts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blogposts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blogposts_AuthorId",
                table: "Blogposts",
                column: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blogposts");
        }
    }
}
