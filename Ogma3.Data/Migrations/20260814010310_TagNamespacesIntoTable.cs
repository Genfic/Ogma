using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20260814010310_TagNamespacesIntoTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Tags_Name_Namespace",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "Namespace",
            table: "Tags");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .Annotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .Annotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
            .Annotation("Npgsql:Enum:e_deleted_by", "staff,user")
            .Annotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,new_follower,system,watched_story_updated,watched_thread_new_comment")
            .Annotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
            .Annotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
            .Annotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
            .Annotation("Npgsql:PostgresExtension:citext", ",,")
            .Annotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,")
            .Annotation("Npgsql:PostgresExtension:intarray", ",,")
            .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
            .Annotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
            .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
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
            .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.AddColumn<long>(
            name: "NamespaceId",
            table: "Tags",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "TagNamespaces",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Slug = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Alias = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                Color = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TagNamespaces", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "TagNamespaces",
            columns: new[] { "Id", "Alias", "Color", "Description", "Name", "Slug" },
            values: new object[,]
            {
                { 1L, "cw", "d91919", null, "Content Warning", "content-warning" },
                { 2L, "ge", "8c37f4", null, "Genre", "genre" },
                { 3L, "fr", "18f900", null, "Franchise", "franchise" }
            });

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name_NamespaceId",
            table: "Tags",
            columns: new[] { "Name", "NamespaceId" },
            unique: true)
            .Annotation("Npgsql:NullsDistinct", false);

        migrationBuilder.CreateIndex(
            name: "IX_Tags_NamespaceId",
            table: "Tags",
            column: "NamespaceId");

        migrationBuilder.CreateIndex(
            name: "IX_TagNamespaces_Name",
            table: "TagNamespaces",
            column: "Name",
            unique: true)
            .Annotation("Relational:Collation", new[] { "nocase-noaccent" });

        migrationBuilder.CreateIndex(
            name: "IX_TagNamespaces_Slug",
            table: "TagNamespaces",
            column: "Slug",
            unique: true)
            .Annotation("Relational:Collation", new[] { "nocase-noaccent" });

        migrationBuilder.AddForeignKey(
            name: "FK_Tags_TagNamespaces_NamespaceId",
            table: "Tags",
            column: "NamespaceId",
            principalTable: "TagNamespaces",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Tags_TagNamespaces_NamespaceId",
            table: "Tags");

        migrationBuilder.DropTable(
            name: "TagNamespaces");

        migrationBuilder.DropIndex(
            name: "IX_Tags_Name_NamespaceId",
            table: "Tags");

        migrationBuilder.DropIndex(
            name: "IX_Tags_NamespaceId",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "NamespaceId",
            table: "Tags");

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
            .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
            .OldAnnotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .OldAnnotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .OldAnnotation("Npgsql:Enum:e_club_member_roles", "admin,founder,moderator,user")
            .OldAnnotation("Npgsql:Enum:e_deleted_by", "staff,user")
            .OldAnnotation("Npgsql:Enum:e_notification_event", "comment_reply,followed_author_new_blogpost,followed_author_new_story,new_follower,system,watched_story_updated,watched_thread_new_comment")
            .OldAnnotation("Npgsql:Enum:e_story_status", "cancelled,completed,in_progress,on_hiatus,unspecified")
            .OldAnnotation("Npgsql:Enum:infraction_type", "ban,mute,note,warning")
            .OldAnnotation("Npgsql:Enum:report_status", "in_review,open,rejected,resolved")
            .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:intarray", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:tsm_system_rows", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,")
            .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.AddColumn<int>(
            name: "Namespace",
            table: "Tags",
            type: "e_tag_namespace",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name_Namespace",
            table: "Tags",
            columns: new[] { "Name", "Namespace" },
            unique: true)
            .Annotation("Npgsql:NullsDistinct", false);
    }
}
