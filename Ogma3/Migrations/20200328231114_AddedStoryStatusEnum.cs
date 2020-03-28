using Microsoft.EntityFrameworkCore.Migrations;
using Ogma3.Data.Enums;

namespace Ogma3.Migrations
{
    public partial class AddedStoryStatusEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled");

            migrationBuilder.AddColumn<EStoryStatus>(
                name: "Status",
                table: "Stories",
                nullable: false,
                defaultValue: EStoryStatus.InProgress);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Stories");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled");
        }
    }
}
