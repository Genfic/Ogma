using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ogma3.Data.Infractions;

namespace Ogma3.Migrations
{
    public partial class NewInfractionsSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .Annotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .Annotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .Annotation("Npgsql:Enum:infraction_type", "note,warning,mute,ban")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .OldAnnotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "Infractions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActiveUntil = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<InfractionType>(type: "infraction_type", nullable: false),
                    IssuedById = table.Column<long>(type: "bigint", nullable: false),
                    RemovedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infractions_AspNetUsers_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Infractions_AspNetUsers_RemovedById",
                        column: x => x.RemovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Infractions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Infractions_IssuedById",
                table: "Infractions",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_Infractions_RemovedById",
                table: "Infractions",
                column: "RemovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Infractions_UserId",
                table: "Infractions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Infractions");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .Annotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .Annotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,comment_reply")
                .OldAnnotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .OldAnnotation("Npgsql:Enum:e_tag_namespace", "content_warning,genre,franchise")
                .OldAnnotation("Npgsql:Enum:infraction_type", "note,warning,mute,ban")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AlterColumn<string[]>(
                name: "Hashtags",
                table: "Blogposts",
                type: "text[]",
                maxLength: 10,
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldMaxLength: 10,
                oldDefaultValue: new string[0]);
        }
    }
}
