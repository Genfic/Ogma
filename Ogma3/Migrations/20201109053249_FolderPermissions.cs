using Microsoft.EntityFrameworkCore.Migrations;
using Ogma3.Data.Enums;

namespace Ogma3.Migrations
{
    public partial class FolderPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<EClubMemberRoles[]>(
                name: "AllowedRoles",
                table: "Folders",
                type: "e_club_member_roles[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedRoles",
                table: "Folders");
        }
    }
}
