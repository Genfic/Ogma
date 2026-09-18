using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260918180229_SimplerAccountDeletion : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Blogposts_AspNetUsers_AuthorId",
            table: "Blogposts");

        migrationBuilder.DropForeignKey(
            name: "FK_Comments_AspNetUsers_AuthorId",
            table: "Comments");

        migrationBuilder.DropForeignKey(
            name: "FK_Comments_AspNetUsers_DeletedByUserId",
            table: "Comments");

        migrationBuilder.DropForeignKey(
            name: "FK_Stories_AspNetUsers_AuthorId",
            table: "Stories");

        migrationBuilder.AlterColumn<long>(
            name: "AuthorId",
            table: "Stories",
            type: "bigint",
            nullable: false,
            defaultValue: -1L,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AlterColumn<long>(
            name: "AuthorId",
            table: "Comments",
            type: "bigint",
            nullable: false,
            defaultValue: -1L,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AlterColumn<long>(
            name: "AuthorId",
            table: "Blogposts",
            type: "bigint",
            nullable: false,
            defaultValue: -1L,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AddForeignKey(
            name: "FK_Blogposts_AspNetUsers_AuthorId",
            table: "Blogposts",
            column: "AuthorId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetDefault);

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_AspNetUsers_AuthorId",
            table: "Comments",
            column: "AuthorId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetDefault);

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_AspNetUsers_DeletedByUserId",
            table: "Comments",
            column: "DeletedByUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_Stories_AspNetUsers_AuthorId",
            table: "Stories",
            column: "AuthorId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetDefault);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Blogposts_AspNetUsers_AuthorId",
            table: "Blogposts");

        migrationBuilder.DropForeignKey(
            name: "FK_Comments_AspNetUsers_AuthorId",
            table: "Comments");

        migrationBuilder.DropForeignKey(
            name: "FK_Comments_AspNetUsers_DeletedByUserId",
            table: "Comments");

        migrationBuilder.DropForeignKey(
            name: "FK_Stories_AspNetUsers_AuthorId",
            table: "Stories");

        migrationBuilder.AlterColumn<long>(
            name: "AuthorId",
            table: "Stories",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldDefaultValue: -1L);

        migrationBuilder.AlterColumn<long>(
            name: "AuthorId",
            table: "Comments",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldDefaultValue: -1L);

        migrationBuilder.AlterColumn<long>(
            name: "AuthorId",
            table: "Blogposts",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldDefaultValue: -1L);

        migrationBuilder.AddForeignKey(
            name: "FK_Blogposts_AspNetUsers_AuthorId",
            table: "Blogposts",
            column: "AuthorId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_AspNetUsers_AuthorId",
            table: "Comments",
            column: "AuthorId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_AspNetUsers_DeletedByUserId",
            table: "Comments",
            column: "DeletedByUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Stories_AspNetUsers_AuthorId",
            table: "Stories",
            column: "AuthorId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
