using Microsoft.EntityFrameworkCore.Migrations;
using Ogma3.Data.Reports;

#nullable disable

namespace Ogma3.Data.Migrations
{
    /// <inheritdoc />
    public partial class _20260519211009_AddReportstatus : Migration
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
                .Annotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
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
                .OldAnnotation("Npgsql:PostgresExtension:intarray", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Reports",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Reports",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<ReportStatus>(
                name: "Status",
                table: "Reports",
                type: "report_status",
                nullable: false,
                defaultValue: ReportStatus.Open);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reports");

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
                .OldAnnotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:intarray", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Reports",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Reports",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }
    }
}
