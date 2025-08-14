using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogma3.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedCitextWithCollation : Migration
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
                name: "Slug",
                table: "Tags",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                collation: "nocase",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                collation: "nocase",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                collation: "nocase-noaccent",
                oldClrType: typeof(string),
                oldType: "citext",
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
                .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "citext",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldCollation: "nocase");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "citext",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldCollation: "nocase");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "citext",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldCollation: "nocase-noaccent");
        }
    }
}
