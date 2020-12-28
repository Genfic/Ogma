using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Ogma3.Data.Enums;

namespace Ogma3.Migrations
{
    public partial class ClubConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Clubs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Clubs",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<EClubMemberRoles>(
                name: "Role",
                table: "ClubMembers",
                type: "e_club_member_roles",
                nullable: false,
                defaultValue: EClubMemberRoles.User,
                oldClrType: typeof(EClubMemberRoles),
                oldType: "e_club_member_roles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MemberSince",
                table: "ClubMembers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Clubs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Clubs",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<EClubMemberRoles>(
                name: "Role",
                table: "ClubMembers",
                type: "e_club_member_roles",
                nullable: false,
                oldClrType: typeof(EClubMemberRoles),
                oldType: "e_club_member_roles",
                oldDefaultValue: EClubMemberRoles.User);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MemberSince",
                table: "ClubMembers",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

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
        }
    }
}
