using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class CitextTagName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .Annotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .Annotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .Annotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .OldAnnotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .OldAnnotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "citext",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .Annotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .Annotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .Annotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .OldAnnotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .OldAnnotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 20);
        }
    }
}
