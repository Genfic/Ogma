using Microsoft.EntityFrameworkCore.Migrations;
using Ogma3.Data.Clubs;

namespace Ogma3.Migrations
{
    public partial class FolderPermissions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedRoles",
                table: "Folders");

            migrationBuilder.AddColumn<EClubMemberRoles>(
                name: "AccessLevel",
                table: "Folders",
                type: "e_club_member_roles",
                nullable: false,
                defaultValue: EClubMemberRoles.User);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessLevel",
                table: "Folders");

            migrationBuilder.AddColumn<EClubMemberRoles[]>(
                name: "AllowedRoles",
                table: "Folders",
                type: "e_club_member_roles[]",
                nullable: true);
        }
    }
}
