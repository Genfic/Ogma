using Microsoft.EntityFrameworkCore.Migrations;
using Ogma3.Data.Clubs;

namespace Ogma3.Migrations
{
    public partial class FixedEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<EClubMemberRoles>(
                name: "Role",
                table: "ClubMembers",
                type: "e_club_member_roles",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "ClubMembers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(EClubMemberRoles),
                oldType: "e_club_member_roles");
        }
    }
}
