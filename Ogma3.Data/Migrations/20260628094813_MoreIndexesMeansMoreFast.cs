using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260628094813_MoreIndexesMeansMoreFast : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Chapters_CreationDate",
            table: "Chapters",
            column: "CreationDate");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_PublicationDate",
            table: "Chapters",
            column: "PublicationDate");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_CreationDate",
            table: "Blogposts",
            column: "CreationDate");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_PublicationDate",
            table: "Blogposts",
            column: "PublicationDate");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Chapters_CreationDate",
            table: "Chapters");

        migrationBuilder.DropIndex(
            name: "IX_Chapters_PublicationDate",
            table: "Chapters");

        migrationBuilder.DropIndex(
            name: "IX_Blogposts_CreationDate",
            table: "Blogposts");

        migrationBuilder.DropIndex(
            name: "IX_Blogposts_PublicationDate",
            table: "Blogposts");
    }
}
