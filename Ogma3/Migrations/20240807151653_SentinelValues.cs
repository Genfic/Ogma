using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class SentinelValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "invalid,founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .Annotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .Annotation("Npgsql:Enum:e_story_status", "unspecified,in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .Annotation("Npgsql:Enum:infraction_type", "note,warning,mute,ban")
                .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .OldAnnotation("Npgsql:Enum:e_story_status", "unspecified,in_progress,completed,on_hiatus,cancelled")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .OldAnnotation("Npgsql:Enum:infraction_type", "note,warning,mute,ban")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LastActive",
                table: "AspNetUsers",
                column: "LastActive",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LastActive",
                table: "AspNetUsers");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .Annotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .Annotation("Npgsql:Enum:e_story_status", "unspecified,in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .Annotation("Npgsql:Enum:infraction_type", "note,warning,mute,ban")
                .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "invalid,founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .OldAnnotation("Npgsql:Enum:e_story_status", "unspecified,in_progress,completed,on_hiatus,cancelled")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .OldAnnotation("Npgsql:Enum:infraction_type", "note,warning,mute,ban")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
