using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260819232856_ProperNewsSystem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "NewsId",
            table: "CommentThreads",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "News",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false, collation: "nocase"),
                Body = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                Slug = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                ExcerptCutoff = table.Column<int>(type: "integer", nullable: false, defaultValue: 200),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                PublicationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                AuthorId = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_News", x => x.Id);
                table.ForeignKey(
                    name: "FK_News_AspNetUsers_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetDefault);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_NewsId",
            table: "CommentThreads",
            column: "NewsId",
            unique: true,
            filter: "\"NewsId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_News_AuthorId",
            table: "News",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_News_PublicationDate",
            table: "News",
            column: "PublicationDate");

        migrationBuilder.CreateIndex(
            name: "IX_News_Title",
            table: "News",
            column: "Title");

        migrationBuilder.AddForeignKey(
            name: "FK_CommentThreads_News_NewsId",
            table: "CommentThreads",
            column: "NewsId",
            principalTable: "News",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CommentThreads_News_NewsId",
            table: "CommentThreads");

        migrationBuilder.DropTable(
            name: "News");

        migrationBuilder.DropIndex(
            name: "IX_CommentThreads_NewsId",
            table: "CommentThreads");

        migrationBuilder.DropColumn(
            name: "NewsId",
            table: "CommentThreads");
    }
}
