using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class ContentBlocks2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogposts_ContentBlock_ContentBlockId",
                table: "Blogposts");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_ContentBlock_ContentBlockId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentBlock_AspNetUsers_IssuerId",
                table: "ContentBlock");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_ContentBlock_ContentBlockId",
                table: "Stories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentBlock",
                table: "ContentBlock");

            migrationBuilder.RenameTable(
                name: "ContentBlock",
                newName: "ContentBlocks");

            migrationBuilder.RenameIndex(
                name: "IX_ContentBlock_IssuerId",
                table: "ContentBlocks",
                newName: "IX_ContentBlocks_IssuerId");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ContentBlocks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "IssuerId",
                table: "ContentBlocks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentBlocks",
                table: "ContentBlocks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogposts_ContentBlocks_ContentBlockId",
                table: "Blogposts",
                column: "ContentBlockId",
                principalTable: "ContentBlocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_ContentBlocks_ContentBlockId",
                table: "Chapters",
                column: "ContentBlockId",
                principalTable: "ContentBlocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentBlocks_AspNetUsers_IssuerId",
                table: "ContentBlocks",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_ContentBlocks_ContentBlockId",
                table: "Stories",
                column: "ContentBlockId",
                principalTable: "ContentBlocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogposts_ContentBlocks_ContentBlockId",
                table: "Blogposts");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_ContentBlocks_ContentBlockId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentBlocks_AspNetUsers_IssuerId",
                table: "ContentBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_ContentBlocks_ContentBlockId",
                table: "Stories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentBlocks",
                table: "ContentBlocks");

            migrationBuilder.RenameTable(
                name: "ContentBlocks",
                newName: "ContentBlock");

            migrationBuilder.RenameIndex(
                name: "IX_ContentBlocks_IssuerId",
                table: "ContentBlock",
                newName: "IX_ContentBlock_IssuerId");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ContentBlock",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "IssuerId",
                table: "ContentBlock",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentBlock",
                table: "ContentBlock",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogposts_ContentBlock_ContentBlockId",
                table: "Blogposts",
                column: "ContentBlockId",
                principalTable: "ContentBlock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_ContentBlock_ContentBlockId",
                table: "Chapters",
                column: "ContentBlockId",
                principalTable: "ContentBlock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentBlock_AspNetUsers_IssuerId",
                table: "ContentBlock",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_ContentBlock_ContentBlockId",
                table: "Stories",
                column: "ContentBlockId",
                principalTable: "ContentBlock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
