using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260731224828_SearchFriendlyUserNameIndexingAndSearchExtensions : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .Annotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .Annotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
            .Annotation("Npgsql:Enum:e_deleted_by", "staff,user")
            .Annotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,new_follower,system,watched_story_updated,watched_thread_new_comment")
            .Annotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
            .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
            .Annotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
            .Annotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
            .Annotation("Npgsql:PostgresExtension:citext", ",,")
            .Annotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,")
            .Annotation("Npgsql:PostgresExtension:intarray", ",,")
            .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
            .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
            .OldAnnotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .OldAnnotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .OldAnnotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
            .OldAnnotation("Npgsql:Enum:e_deleted_by", "staff,user")
            .OldAnnotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,new_follower,system,watched_story_updated,watched_thread_new_comment")
            .OldAnnotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
            .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
            .OldAnnotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
            .OldAnnotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
            .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:intarray", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_NormalizedUserName_Trgm",
            table: "AspNetUsers",
            column: "NormalizedUserName")
            .Annotation("Npgsql:IndexMethod", "gin")
            .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_NormalizedUserName_Trgm",
            table: "AspNetUsers");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .Annotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .Annotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
            .Annotation("Npgsql:Enum:e_deleted_by", "staff,user")
            .Annotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,new_follower,system,watched_story_updated,watched_thread_new_comment")
            .Annotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
            .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
            .Annotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
            .Annotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
            .Annotation("Npgsql:PostgresExtension:citext", ",,")
            .Annotation("Npgsql:PostgresExtension:intarray", ",,")
            .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
            .OldAnnotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .OldAnnotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .OldAnnotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
            .OldAnnotation("Npgsql:Enum:e_deleted_by", "staff,user")
            .OldAnnotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,new_follower,system,watched_story_updated,watched_thread_new_comment")
            .OldAnnotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
            .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,franchise,genre")
            .OldAnnotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
            .OldAnnotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
            .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:intarray", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
    }
}
