using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class _20260420192737_ChapterMinHashSignatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
                .Annotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
                .Annotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .Annotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .Annotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .Annotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:intarray", ",,")
                .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
                .OldAnnotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .OldAnnotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .OldAnnotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AddColumn<int[]>(
                name: "Signature",
                table: "Chapters",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_Signature",
                table: "Chapters",
                column: "Signature")
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chapters_Signature",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Chapters");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
                .Annotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
                .Annotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .Annotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .Annotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .Annotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
                .OldAnnotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "staff,user")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,system,watched_story_updated,watched_thread_new_comment")
                .OldAnnotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
                .OldAnnotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:intarray", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
