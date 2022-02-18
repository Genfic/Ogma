using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    public partial class ClubBans2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubBan_AspNetUsers_IssuerId",
                table: "ClubBan");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubBan_AspNetUsers_UserId",
                table: "ClubBan");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubBan_Clubs_ClubId",
                table: "ClubBan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubBan",
                table: "ClubBan");

            migrationBuilder.RenameTable(
                name: "ClubBan",
                newName: "ClubBans");

            migrationBuilder.RenameIndex(
                name: "IX_ClubBan_UserId",
                table: "ClubBans",
                newName: "IX_ClubBans_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ClubBan_IssuerId",
                table: "ClubBans",
                newName: "IX_ClubBans_IssuerId");

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
                name: "PK_ClubBans",
                table: "ClubBans",
                columns: new[] { "ClubId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBans_AspNetUsers_IssuerId",
                table: "ClubBans",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBans_AspNetUsers_UserId",
                table: "ClubBans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBans_Clubs_ClubId",
                table: "ClubBans",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubBans_AspNetUsers_IssuerId",
                table: "ClubBans");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubBans_AspNetUsers_UserId",
                table: "ClubBans");

            migrationBuilder.DropForeignKey(
                name: "FK_ClubBans_Clubs_ClubId",
                table: "ClubBans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClubBans",
                table: "ClubBans");

            migrationBuilder.RenameTable(
                name: "ClubBans",
                newName: "ClubBan");

            migrationBuilder.RenameIndex(
                name: "IX_ClubBans_UserId",
                table: "ClubBan",
                newName: "IX_ClubBan_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ClubBans_IssuerId",
                table: "ClubBan",
                newName: "IX_ClubBan_IssuerId");

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
                name: "PK_ClubBan",
                table: "ClubBan",
                columns: new[] { "ClubId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBan_AspNetUsers_IssuerId",
                table: "ClubBan",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBan_AspNetUsers_UserId",
                table: "ClubBan",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubBan_Clubs_ClubId",
                table: "ClubBan",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
