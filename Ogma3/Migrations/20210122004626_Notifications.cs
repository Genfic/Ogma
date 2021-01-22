using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ogma3.Data.Enums;

namespace Ogma3.Migrations
{
    public partial class Notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .Annotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,followed_thread_new_comment,comment_reply")
                .Annotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .OldAnnotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
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
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Event = table.Column<ENotificationEvent>(type: "e_notification_event", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRecipients",
                columns: table => new
                {
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    NotificationId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipients", x => new { x.NotificationId, x.RecipientId });
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_AspNetUsers_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_RecipientId",
                table: "NotificationRecipients",
                column: "RecipientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .Annotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .Annotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:e_club_member_roles", "founder,admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:e_deleted_by", "user,staff")
                .OldAnnotation("Npgsql:Enum:e_notification_event", "system,watched_story_updated,watched_thread_new_comment,followed_author_new_blogpost,followed_author_new_story,followed_thread_new_comment,comment_reply")
                .OldAnnotation("Npgsql:Enum:e_story_status", "in_progress,completed,on_hiatus,cancelled")
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
