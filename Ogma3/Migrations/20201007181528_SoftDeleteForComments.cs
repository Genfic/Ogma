using Microsoft.EntityFrameworkCore.Migrations;

namespace Ogma3.Migrations
{
    public partial class SoftDeleteForComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .Annotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Comments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedByUserId",
                table: "Comments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DeletedByUserId",
                table: "Comments",
                column: "DeletedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_DeletedByUserId",
                table: "Comments",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_DeletedByUserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_DeletedByUserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Comments");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .OldAnnotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
