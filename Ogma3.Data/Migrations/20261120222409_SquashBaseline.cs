using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.Infractions;
using Ogma3.Data.Notifications;
using Ogma3.Data.Reports;
using Ogma3.Data.Stories;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ogma3.Data.Migrations;

/// <inheritdoc />
public partial class _20261120222409_SquashBaseline : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .Annotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .Annotation("Npgsql:CollationSchemasResolved", true)
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
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, collation: "nocase"),
                NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                IsStaff = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                Order = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Documents",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Slug = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                RevisionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                Body = table.Column<string>(type: "text", nullable: false),
                CompiledBody = table.Column<string>(type: "text", nullable: false),
                CustomCss = table.Column<string>(type: "text", nullable: true),
                CustomJs = table.Column<string>(type: "text", nullable: true),
                Headers = table.Column<string>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Documents", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Faqs",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Question = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                Answer = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                AnswerRendered = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Faqs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Icons",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Icons", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Images",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                ETag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Images", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Body = table.Column<string>(type: "text", nullable: true),
                Url = table.Column<string>(type: "text", nullable: false),
                DateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Event = table.Column<ENotificationEvent>(type: "e_notification_event", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Quotes",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Body = table.Column<string>(type: "text", nullable: false),
                Author = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Quotes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Ratings",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "nocase-noaccent"),
                Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                Order = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                Color = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                BlacklistedByDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Ratings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SubscriptionTiers",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                AmountCents = table.Column<int>(type: "integer", nullable: false),
                Entitlements = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SubscriptionTiers", x => x.Id);
            });

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

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<long>(type: "bigint", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                NormalizedUserName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "nocase-noaccent"),
                Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                NormalizedEmail = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                Title = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                Bio = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                Links = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                AvatarId = table.Column<long>(type: "bigint", nullable: false),
                RegistrationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                LastActive = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                SubscriptionId = table.Column<long>(type: "bigint", nullable: true),
                TimeZone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "UTC"),
                PrivacyPolicyAcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                TosAcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                SafetyPinHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                SafetyPinResetTokenHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                SafetyPinResetTokenExpiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                SafetyPinLockedOutUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: true),
                SecurityStamp = table.Column<string>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                table.CheckConstraint("CK_OgmaUser_Links_cardinality", "cardinality(\"Links\") <= 5");
                table.ForeignKey(
                    name: "FK_AspNetUsers_Images_AvatarId",
                    column: x => x.AvatarId,
                    principalTable: "Images",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Clubs",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Hook = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(25000)", maxLength: 25000, nullable: false),
                IconId = table.Column<long>(type: "bigint", nullable: false),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Clubs", x => x.Id);
                table.ForeignKey(
                    name: "FK_Clubs_Images_IconId",
                    column: x => x.IconId,
                    principalTable: "Images",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                UserId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserPasskeys",
            columns: table => new
            {
                CredentialId = table.Column<byte[]>(type: "bytea", maxLength: 1024, nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                Data = table.Column<string>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserPasskeys", x => x.CredentialId);
                table.ForeignKey(
                    name: "FK_AspNetUserPasskeys_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                RoleId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new
            {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "BlacklistedRatings",
            columns: table => new
            {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                RatingId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlacklistedRatings", x => new { x.UserId, x.RatingId });
                table.ForeignKey(
                    name: "FK_BlacklistedRatings_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_BlacklistedRatings_Ratings_RatingId",
                    column: x => x.RatingId,
                    principalTable: "Ratings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "BlockedUsers",
            columns: table => new
            {
                BlockingUserId = table.Column<long>(type: "bigint", nullable: false),
                BlockedUserId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlockedUsers", x => new { x.BlockingUserId, x.BlockedUserId });
                table.ForeignKey(
                    name: "FK_BlockedUsers_AspNetUsers_BlockedUserId",
                    column: x => x.BlockedUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_BlockedUsers_AspNetUsers_BlockingUserId",
                    column: x => x.BlockingUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ContentBlocks",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                IssuerId = table.Column<long>(type: "bigint", nullable: false),
                Reason = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                DateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContentBlocks", x => x.Id);
                table.ForeignKey(
                    name: "FK_ContentBlocks_AspNetUsers_IssuerId",
                    column: x => x.IssuerId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "FollowedUsers",
            columns: table => new
            {
                FollowingUserId = table.Column<long>(type: "bigint", nullable: false),
                FollowedUserId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FollowedUsers", x => new { x.FollowingUserId, x.FollowedUserId });
                table.ForeignKey(
                    name: "FK_FollowedUsers_AspNetUsers_FollowedUserId",
                    column: x => x.FollowedUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FollowedUsers_AspNetUsers_FollowingUserId",
                    column: x => x.FollowingUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GlobalNotifications",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Message = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ArchivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Color = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                CreatedById = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GlobalNotifications", x => x.Id);
                table.ForeignKey(
                    name: "FK_GlobalNotifications_AspNetUsers_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Infractions",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                IssueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                ActiveUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                RemovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
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
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Infractions_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "InviteCodes",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "nocase-noaccent"),
                UsedById = table.Column<long>(type: "bigint", nullable: true),
                IssuedById = table.Column<long>(type: "bigint", nullable: true),
                IssuedByType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                UsedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IssueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InviteCodes", x => x.Id);
                table.ForeignKey(
                    name: "FK_InviteCodes_AspNetUsers_IssuedById",
                    column: x => x.IssuedById,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_InviteCodes_AspNetUsers_UsedById",
                    column: x => x.UsedById,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ModeratorActions",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                StaffMemberId = table.Column<long>(type: "bigint", nullable: false),
                Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                DateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ModeratorActions", x => x.Id);
                table.ForeignKey(
                    name: "FK_ModeratorActions_AspNetUsers_StaffMemberId",
                    column: x => x.StaffMemberId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "News",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false, collation: "nocase"),
                Body = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                Slug = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                ExcerptCutoff = table.Column<int>(type: "integer", nullable: false, defaultValue: 200),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                PublicationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                CommitHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "CAFEBABE"),
                AuthorId = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_News", x => x.Id);
                table.ForeignKey(
                    name: "FK_News_AspNetUsers_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetDefault);
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

        migrationBuilder.CreateTable(
            name: "Shelves",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: ""),
                OwnerId = table.Column<long>(type: "bigint", nullable: false),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsQuickAdd = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                TrackUpdates = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                IconId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Shelves", x => x.Id);
                table.ForeignKey(
                    name: "FK_Shelves_AspNetUsers_OwnerId",
                    column: x => x.OwnerId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Shelves_Icons_IconId",
                    column: x => x.IconId,
                    principalTable: "Icons",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "Subscriptions",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                TierId = table.Column<long>(type: "bigint", nullable: true),
                PatreonTierIds = table.Column<List<string>>(type: "text[]", nullable: false),
                PatreonStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                PatreonUserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                LastChange = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Subscriptions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Subscriptions_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Subscriptions_SubscriptionTiers_TierId",
                    column: x => x.TierId,
                    principalTable: "SubscriptionTiers",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Tags",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false, collation: "nocase"),
                Slug = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false, collation: "nocase"),
                Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                NamespaceId = table.Column<long>(type: "bigint", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                LastChange = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedById = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tags", x => x.Id);
                table.ForeignKey(
                    name: "FK_Tags_AspNetUsers_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Tags_TagNamespaces_NamespaceId",
                    column: x => x.NamespaceId,
                    principalTable: "TagNamespaces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "ClubMembers",
            columns: table => new
            {
                MemberId = table.Column<long>(type: "bigint", nullable: false),
                ClubId = table.Column<long>(type: "bigint", nullable: false),
                Role = table.Column<EClubMemberRoles>(type: "e_club_member_roles", nullable: false, defaultValue: EClubMemberRoles.User),
                MemberSince = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClubMembers", x => new { x.ClubId, x.MemberId });
                table.ForeignKey(
                    name: "FK_ClubMembers_AspNetUsers_MemberId",
                    column: x => x.MemberId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ClubMembers_Clubs_ClubId",
                    column: x => x.ClubId,
                    principalTable: "Clubs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ClubModeratorActions",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ModeratorId = table.Column<long>(type: "bigint", nullable: true),
                Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                ClubId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClubModeratorActions", x => x.Id);
                table.ForeignKey(
                    name: "FK_ClubModeratorActions_AspNetUsers_ModeratorId",
                    column: x => x.ModeratorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_ClubModeratorActions_Clubs_ClubId",
                    column: x => x.ClubId,
                    principalTable: "Clubs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ClubThreads",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Body = table.Column<string>(type: "character varying(25000)", maxLength: 25000, nullable: false),
                AuthorId = table.Column<long>(type: "bigint", nullable: false),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                ClubId = table.Column<long>(type: "bigint", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsPinned = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClubThreads", x => x.Id);
                table.ForeignKey(
                    name: "FK_ClubThreads_AspNetUsers_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_ClubThreads_Clubs_ClubId",
                    column: x => x.ClubId,
                    principalTable: "Clubs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Folders",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Slug = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ClubId = table.Column<long>(type: "bigint", nullable: false),
                AccessLevel = table.Column<EClubMemberRoles>(type: "e_club_member_roles", nullable: false, defaultValue: EClubMemberRoles.User)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Folders", x => x.Id);
                table.ForeignKey(
                    name: "FK_Folders_Clubs_ClubId",
                    column: x => x.ClubId,
                    principalTable: "Clubs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Stories",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AuthorId = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "nocase"),
                Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                Hook = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                CoverId = table.Column<long>(type: "bigint", nullable: true),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                PublicationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ScheduledFor = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LastUpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                VoteCount = table.Column<int>(type: "integer", nullable: false),
                ExtraTags = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                RatingId = table.Column<long>(type: "bigint", nullable: false),
                Status = table.Column<EStoryStatus>(type: "e_story_status", nullable: false, defaultValue: EStoryStatus.InProgress),
                WordCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                ChapterCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                ScheduledForDeletion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ContentBlockId = table.Column<long>(type: "bigint", nullable: true),
                Credits = table.Column<string>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Stories", x => x.Id);
                table.ForeignKey(
                    name: "FK_Stories_AspNetUsers_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetDefault);
                table.ForeignKey(
                    name: "FK_Stories_ContentBlocks_ContentBlockId",
                    column: x => x.ContentBlockId,
                    principalTable: "ContentBlocks",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Stories_Images_CoverId",
                    column: x => x.CoverId,
                    principalTable: "Images",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Stories_Ratings_RatingId",
                    column: x => x.RatingId,
                    principalTable: "Ratings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "BlacklistedTags",
            columns: table => new
            {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                TagId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlacklistedTags", x => new { x.UserId, x.TagId });
                table.ForeignKey(
                    name: "FK_BlacklistedTags_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_BlacklistedTags_Tags_TagId",
                    column: x => x.TagId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Chapters",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Order = table.Column<long>(type: "bigint", nullable: false),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                PublicationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ScheduledFor = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Body = table.Column<string>(type: "character varying(500000)", maxLength: 500000, nullable: false),
                StartNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                EndNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                WordCount = table.Column<int>(type: "integer", nullable: false),
                StoryId = table.Column<long>(type: "bigint", nullable: false),
                ContentBlockId = table.Column<long>(type: "bigint", nullable: true),
                Signature = table.Column<int[]>(type: "integer[]", nullable: false),
                ScheduledForDeletion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Chapters", x => x.Id);
                table.ForeignKey(
                    name: "FK_Chapters_ContentBlocks_ContentBlockId",
                    column: x => x.ContentBlockId,
                    principalTable: "ContentBlocks",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Chapters_Stories_StoryId",
                    column: x => x.StoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ChaptersRead",
            columns: table => new
            {
                StoryId = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                Chapters = table.Column<List<long>>(type: "bigint[]", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChaptersRead", x => new { x.StoryId, x.UserId });
                table.ForeignKey(
                    name: "FK_ChaptersRead_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ChaptersRead_Stories_StoryId",
                    column: x => x.StoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "FolderStories",
            columns: table => new
            {
                FolderId = table.Column<long>(type: "bigint", nullable: false),
                StoryId = table.Column<long>(type: "bigint", nullable: false),
                Added = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                AddedById = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FolderStories", x => new { x.FolderId, x.StoryId });
                table.ForeignKey(
                    name: "FK_FolderStories_AspNetUsers_AddedById",
                    column: x => x.AddedById,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FolderStories_Folders_FolderId",
                    column: x => x.FolderId,
                    principalTable: "Folders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FolderStories_Stories_StoryId",
                    column: x => x.StoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ShelfStories",
            columns: table => new
            {
                ShelfId = table.Column<long>(type: "bigint", nullable: false),
                StoryId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShelfStories", x => new { x.ShelfId, x.StoryId });
                table.ForeignKey(
                    name: "FK_ShelfStories_Shelves_ShelfId",
                    column: x => x.ShelfId,
                    principalTable: "Shelves",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ShelfStories_Stories_StoryId",
                    column: x => x.StoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "StoryTags",
            columns: table => new
            {
                StoryId = table.Column<long>(type: "bigint", nullable: false),
                TagId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StoryTags", x => new { x.StoryId, x.TagId });
                table.ForeignKey(
                    name: "FK_StoryTags_Stories_StoryId",
                    column: x => x.StoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_StoryTags_Tags_TagId",
                    column: x => x.TagId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Votes",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                StoryId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Votes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Votes_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Votes_Stories_StoryId",
                    column: x => x.StoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Blogposts",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "nocase"),
                Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ExcerptCutoff = table.Column<int>(type: "integer", nullable: false, defaultValue: 200),
                PublicationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ScheduledFor = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                AuthorId = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                Body = table.Column<string>(type: "character varying(500000)", maxLength: 500000, nullable: false),
                WordCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                Hashtags = table.Column<string[]>(type: "character varying(20)[]", nullable: false, defaultValue: new string[0]),
                IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                AttachedStoryId = table.Column<long>(type: "bigint", nullable: true),
                AttachedChapterId = table.Column<long>(type: "bigint", nullable: true),
                ContentBlockId = table.Column<long>(type: "bigint", nullable: true),
                ScheduledForDeletion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Blogposts", x => x.Id);
                table.CheckConstraint("CK_Blogpost_Hashtags_cardinality", "cardinality(\"Hashtags\") <= 20");
                table.ForeignKey(
                    name: "FK_Blogposts_AspNetUsers_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetDefault);
                table.ForeignKey(
                    name: "FK_Blogposts_Chapters_AttachedChapterId",
                    column: x => x.AttachedChapterId,
                    principalTable: "Chapters",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Blogposts_ContentBlocks_ContentBlockId",
                    column: x => x.ContentBlockId,
                    principalTable: "ContentBlocks",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Blogposts_Stories_AttachedStoryId",
                    column: x => x.AttachedStoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "CommentThreads",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CommentsCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                LockDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsLocked = table.Column<bool>(type: "boolean", nullable: false, computedColumnSql: "\"LockDate\" IS NOT NULL", stored: true),
                LastChange = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Source = table.Column<short>(type: "smallint", nullable: false, computedColumnSql: "CASE\r\n	WHEN \"ChapterId\" IS NOT NULL THEN 0\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN 1\r\n	WHEN \"UserId\" IS NOT NULL THEN 2\r\n	WHEN \"ClubThreadId\" IS NOT NULL THEN 3\r\n	WHEN \"NewsId\" IS NOT NULL THEN 4\r\nEND", stored: true),
                SourceId = table.Column<long>(type: "bigint", nullable: false, computedColumnSql: "CASE\r\n	WHEN \"ChapterId\" IS NOT NULL THEN \"ChapterId\"\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN \"BlogpostId\"\r\n	WHEN \"UserId\" IS NOT NULL THEN \"UserId\"\r\n	WHEN \"ClubThreadId\" IS NOT NULL THEN \"ClubThreadId\"\r\n	WHEN \"NewsId\" IS NOT NULL THEN \"NewsId\"\r\nEND", stored: true),
                UserId = table.Column<long>(type: "bigint", nullable: true),
                ChapterId = table.Column<long>(type: "bigint", nullable: true),
                BlogpostId = table.Column<long>(type: "bigint", nullable: true),
                ClubThreadId = table.Column<long>(type: "bigint", nullable: true),
                NewsId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CommentThreads", x => x.Id);
                table.ForeignKey(
                    name: "FK_CommentThreads_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CommentThreads_Blogposts_BlogpostId",
                    column: x => x.BlogpostId,
                    principalTable: "Blogposts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CommentThreads_Chapters_ChapterId",
                    column: x => x.ChapterId,
                    principalTable: "Chapters",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CommentThreads_ClubThreads_ClubThreadId",
                    column: x => x.ClubThreadId,
                    principalTable: "ClubThreads",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CommentThreads_News_NewsId",
                    column: x => x.NewsId,
                    principalTable: "News",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Comments",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CommentsThreadId = table.Column<long>(type: "bigint", nullable: false),
                AuthorId = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                DateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Body = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                DeletedBy = table.Column<EDeletedBy>(type: "e_deleted_by", nullable: true),
                DeletedByUserId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Comments_AspNetUsers_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetDefault);
                table.ForeignKey(
                    name: "FK_Comments_AspNetUsers_DeletedByUserId",
                    column: x => x.DeletedByUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Comments_CommentThreads_CommentsThreadId",
                    column: x => x.CommentsThreadId,
                    principalTable: "CommentThreads",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CommentThreadSubscribers",
            columns: table => new
            {
                CommentsThreadId = table.Column<long>(type: "bigint", nullable: false),
                OgmaUserId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CommentThreadSubscribers", x => new { x.CommentsThreadId, x.OgmaUserId });
                table.ForeignKey(
                    name: "FK_CommentThreadSubscribers_AspNetUsers_OgmaUserId",
                    column: x => x.OgmaUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CommentThreadSubscribers_CommentThreads_CommentsThreadId",
                    column: x => x.CommentsThreadId,
                    principalTable: "CommentThreads",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CommentRevisions",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EditTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Body = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                ParentId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CommentRevisions", x => x.Id);
                table.ForeignKey(
                    name: "FK_CommentRevisions_Comments_ParentId",
                    column: x => x.ParentId,
                    principalTable: "Comments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Reports",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ReporterId = table.Column<long>(type: "bigint", nullable: false),
                ReportDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Status = table.Column<ReportStatus>(type: "report_status", nullable: false),
                ContentType = table.Column<short>(type: "smallint", nullable: false, computedColumnSql: "CASE\r\n	WHEN \"ChapterId\"  IS NOT NULL THEN 3\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN 4\r\n   WHEN \"StoryId\"    IS NOT NULL THEN 2\r\n	WHEN \"UserId\"     IS NOT NULL THEN 1\r\n	WHEN \"CommentId\"  IS NOT NULL THEN 0\r\n	WHEN \"ClubId\"     IS NOT NULL THEN 5\r\nEND", stored: true),
                ContentId = table.Column<long>(type: "bigint", nullable: false, computedColumnSql: "CASE\r\n	WHEN \"ChapterId\"  IS NOT NULL THEN \"ChapterId\"\r\n	WHEN \"BlogpostId\" IS NOT NULL THEN \"BlogpostId\"\r\n   WHEN \"StoryId\"    IS NOT NULL THEN \"StoryId\"\r\n	WHEN \"UserId\"     IS NOT NULL THEN \"UserId\"\r\n	WHEN \"CommentId\"  IS NOT NULL THEN \"CommentId\"\r\n	WHEN \"ClubId\"     IS NOT NULL THEN \"ClubId\"\r\nEND", stored: true),
                CommentId = table.Column<long>(type: "bigint", nullable: true),
                UserId = table.Column<long>(type: "bigint", nullable: true),
                StoryId = table.Column<long>(type: "bigint", nullable: true),
                ChapterId = table.Column<long>(type: "bigint", nullable: true),
                BlogpostId = table.Column<long>(type: "bigint", nullable: true),
                ClubId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reports", x => x.Id);
                table.ForeignKey(
                    name: "FK_Reports_AspNetUsers_ReporterId",
                    column: x => x.ReporterId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Reports_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Reports_Blogposts_BlogpostId",
                    column: x => x.BlogpostId,
                    principalTable: "Blogposts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Reports_Chapters_ChapterId",
                    column: x => x.ChapterId,
                    principalTable: "Chapters",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Reports_Clubs_ClubId",
                    column: x => x.ClubId,
                    principalTable: "Clubs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Reports_Comments_CommentId",
                    column: x => x.CommentId,
                    principalTable: "Comments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Reports_Stories_StoryId",
                    column: x => x.StoryId,
                    principalTable: "Stories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "Images",
            columns: new[] { "Id", "ETag", "Url" },
            values: new object[,]
            {
                { -2L, null, "/img/placeholders/anonymous-user.png" },
                { -1L, null, "/img/placeholders/deleted-user.png" }
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

        migrationBuilder.InsertData(
            table: "AspNetUsers",
            columns: new[] { "Id", "AccessFailedCount", "AvatarId", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SafetyPinHash", "SafetyPinLockedOutUntil", "SafetyPinResetTokenExpiry", "SafetyPinResetTokenHash", "SecurityStamp", "SubscriptionId", "Title", "TwoFactorEnabled", "UserName" },
            values: new object[,]
            {
                { -2L, 0, -2L, null, "00000000-0000-0000-0000-000000000002", "", false, false, null, "", "ANONYMOUS USER", null, null, null, null, null, "00000000-0000-0000-0000-000000000002", null, null, false, "Anonymous User" },
                { -1L, 0, -1L, null, "00000000-0000-0000-0000-000000000001", "", false, false, null, "", "DELETED USER", null, null, null, null, null, "00000000-0000-0000-0000-000000000001", null, null, false, "Deleted User" }
            });

        migrationBuilder.InsertData(
            table: "CommentThreads",
            columns: new[] { "Id", "BlogpostId", "ChapterId", "ClubThreadId", "LockDate", "NewsId", "UserId" },
            values: new object[,]
            {
                { -2L, null, null, null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, -2L },
                { -1L, null, null, null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, -1L }
            });

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserPasskeys_UserId",
            table: "AspNetUserPasskeys",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_AvatarId",
            table: "AspNetUsers",
            column: "AvatarId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_LastActive",
            table: "AspNetUsers",
            column: "LastActive",
            descending: new bool[0]);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_NormalizedUserName_Trgm",
            table: "AspNetUsers",
            column: "NormalizedUserName")
            .Annotation("Npgsql:IndexMethod", "gist")
            .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BlacklistedRatings_RatingId",
            table: "BlacklistedRatings",
            column: "RatingId");

        migrationBuilder.CreateIndex(
            name: "IX_BlacklistedTags_TagId",
            table: "BlacklistedTags",
            column: "TagId");

        migrationBuilder.CreateIndex(
            name: "IX_BlockedUsers_BlockedUserId",
            table: "BlockedUsers",
            column: "BlockedUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_AttachedChapterId",
            table: "Blogposts",
            column: "AttachedChapterId");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_AttachedStoryId",
            table: "Blogposts",
            column: "AttachedStoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_AuthorId",
            table: "Blogposts",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_ContentBlockId",
            table: "Blogposts",
            column: "ContentBlockId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_Hashtags",
            table: "Blogposts",
            column: "Hashtags",
            filter: "\"IsVisible\"")
            .Annotation("Npgsql:IndexMethod", "gin");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_PublicationDate",
            table: "Blogposts",
            column: "PublicationDate");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_ScheduledFor",
            table: "Blogposts",
            column: "ScheduledFor",
            filter: "\"ScheduledFor\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_ScheduledForDeletion",
            table: "Blogposts",
            column: "ScheduledForDeletion",
            filter: "\"ScheduledForDeletion\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Blogposts_Title",
            table: "Blogposts",
            column: "Title");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_ContentBlockId",
            table: "Chapters",
            column: "ContentBlockId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_CreationDate",
            table: "Chapters",
            column: "CreationDate");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_PublicationDate",
            table: "Chapters",
            column: "PublicationDate");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_ScheduledFor",
            table: "Chapters",
            column: "ScheduledFor",
            filter: "\"ScheduledFor\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_ScheduledForDeletion",
            table: "Chapters",
            column: "ScheduledForDeletion",
            filter: "\"ScheduledForDeletion\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_Signature",
            table: "Chapters",
            column: "Signature")
            .Annotation("Npgsql:IndexMethod", "gin");

        migrationBuilder.CreateIndex(
            name: "IX_Chapters_StoryId",
            table: "Chapters",
            column: "StoryId");

        migrationBuilder.CreateIndex(
            name: "IX_ChaptersRead_UserId",
            table: "ChaptersRead",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_ClubMembers_MemberId",
            table: "ClubMembers",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_ClubModeratorActions_ClubId",
            table: "ClubModeratorActions",
            column: "ClubId");

        migrationBuilder.CreateIndex(
            name: "IX_ClubModeratorActions_ModeratorId",
            table: "ClubModeratorActions",
            column: "ModeratorId");

        migrationBuilder.CreateIndex(
            name: "IX_Clubs_IconId",
            table: "Clubs",
            column: "IconId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ClubThreads_AuthorId",
            table: "ClubThreads",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_ClubThreads_ClubId",
            table: "ClubThreads",
            column: "ClubId");

        migrationBuilder.CreateIndex(
            name: "IX_CommentRevisions_ParentId",
            table: "CommentRevisions",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_Comments_AuthorId",
            table: "Comments",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_Comments_CommentsThreadId",
            table: "Comments",
            column: "CommentsThreadId");

        migrationBuilder.CreateIndex(
            name: "IX_Comments_DeletedByUserId",
            table: "Comments",
            column: "DeletedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_BlogpostId",
            table: "CommentThreads",
            column: "BlogpostId",
            unique: true,
            filter: "\"BlogpostId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_ChapterId",
            table: "CommentThreads",
            column: "ChapterId",
            unique: true,
            filter: "\"ChapterId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_ClubThreadId",
            table: "CommentThreads",
            column: "ClubThreadId",
            unique: true,
            filter: "\"ClubThreadId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_NewsId",
            table: "CommentThreads",
            column: "NewsId",
            unique: true,
            filter: "\"NewsId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreads_UserId",
            table: "CommentThreads",
            column: "UserId",
            unique: true,
            filter: "\"UserId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_CommentThreadSubscribers_OgmaUserId",
            table: "CommentThreadSubscribers",
            column: "OgmaUserId");

        migrationBuilder.CreateIndex(
            name: "IX_ContentBlocks_IssuerId",
            table: "ContentBlocks",
            column: "IssuerId");

        migrationBuilder.CreateIndex(
            name: "IX_Documents_Slug_Version",
            table: "Documents",
            columns: new[] { "Slug", "Version" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Documents_Title_Version",
            table: "Documents",
            columns: new[] { "Title", "Version" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Folders_ClubId",
            table: "Folders",
            column: "ClubId");

        migrationBuilder.CreateIndex(
            name: "IX_FolderStories_AddedById",
            table: "FolderStories",
            column: "AddedById");

        migrationBuilder.CreateIndex(
            name: "IX_FolderStories_StoryId",
            table: "FolderStories",
            column: "StoryId");

        migrationBuilder.CreateIndex(
            name: "IX_FollowedUsers_FollowedUserId",
            table: "FollowedUsers",
            column: "FollowedUserId");

        migrationBuilder.CreateIndex(
            name: "IX_GlobalNotifications_CreatedById",
            table: "GlobalNotifications",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Icons_Name",
            table: "Icons",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Infractions_IssuedById",
            table: "Infractions",
            column: "IssuedById");

        migrationBuilder.CreateIndex(
            name: "IX_Infractions_RemovedAt",
            table: "Infractions",
            column: "RemovedAt",
            filter: "\"RemovedAt\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Infractions_RemovedById",
            table: "Infractions",
            column: "RemovedById");

        migrationBuilder.CreateIndex(
            name: "IX_Infractions_Type",
            table: "Infractions",
            column: "Type");

        migrationBuilder.CreateIndex(
            name: "IX_Infractions_UserId",
            table: "Infractions",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_InviteCodes_Code",
            table: "InviteCodes",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_InviteCodes_IssuedById",
            table: "InviteCodes",
            column: "IssuedById");

        migrationBuilder.CreateIndex(
            name: "IX_InviteCodes_UsedById",
            table: "InviteCodes",
            column: "UsedById",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ModeratorActions_StaffMemberId",
            table: "ModeratorActions",
            column: "StaffMemberId");

        migrationBuilder.CreateIndex(
            name: "IX_News_AuthorId",
            table: "News",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_News_PublicationDate",
            table: "News",
            column: "PublicationDate");

        migrationBuilder.CreateIndex(
            name: "IX_News_Title",
            table: "News",
            column: "Title");

        migrationBuilder.CreateIndex(
            name: "IX_NotificationRecipients_RecipientId",
            table: "NotificationRecipients",
            column: "RecipientId");

        migrationBuilder.CreateIndex(
            name: "IX_Ratings_Name",
            table: "Ratings",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Reports_BlogpostId",
            table: "Reports",
            column: "BlogpostId");

        migrationBuilder.CreateIndex(
            name: "IX_Reports_ChapterId",
            table: "Reports",
            column: "ChapterId");

        migrationBuilder.CreateIndex(
            name: "IX_Reports_ClubId",
            table: "Reports",
            column: "ClubId");

        migrationBuilder.CreateIndex(
            name: "IX_Reports_CommentId",
            table: "Reports",
            column: "CommentId");

        migrationBuilder.CreateIndex(
            name: "IX_Reports_ReporterId",
            table: "Reports",
            column: "ReporterId");

        migrationBuilder.CreateIndex(
            name: "IX_Reports_StoryId",
            table: "Reports",
            column: "StoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Reports_UserId",
            table: "Reports",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_ShelfStories_StoryId",
            table: "ShelfStories",
            column: "StoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Shelves_IconId",
            table: "Shelves",
            column: "IconId");

        migrationBuilder.CreateIndex(
            name: "IX_Shelves_OwnerId",
            table: "Shelves",
            column: "OwnerId");

        migrationBuilder.CreateIndex(
            name: "IX_Stories_AuthorId",
            table: "Stories",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_Stories_ContentBlockId",
            table: "Stories",
            column: "ContentBlockId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Stories_CoverId",
            table: "Stories",
            column: "CoverId");

        migrationBuilder.CreateIndex(
            name: "IX_Stories_RatingId",
            table: "Stories",
            column: "RatingId");

        migrationBuilder.CreateIndex(
            name: "IX_Stories_ScheduledFor",
            table: "Stories",
            column: "ScheduledFor",
            filter: "\"ScheduledFor\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Stories_ScheduledForDeletion",
            table: "Stories",
            column: "ScheduledForDeletion",
            filter: "\"ScheduledForDeletion\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_StoryTags_TagId",
            table: "StoryTags",
            column: "TagId");

        migrationBuilder.CreateIndex(
            name: "IX_Subscriptions_RevokedAt",
            table: "Subscriptions",
            column: "RevokedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Subscriptions_TierId",
            table: "Subscriptions",
            column: "TierId");

        migrationBuilder.CreateIndex(
            name: "IX_Subscriptions_UserId",
            table: "Subscriptions",
            column: "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TagNamespaces_Alias",
            table: "TagNamespaces",
            column: "Alias",
            unique: true,
            filter: "\"Alias\" IS NOT NULL")
            .Annotation("Relational:Collation", new[] { "nocase-noaccent" });

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

        migrationBuilder.CreateIndex(
            name: "IX_Tags_CreatedById",
            table: "Tags",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_LastChange",
            table: "Tags",
            column: "LastChange",
            filter: "\"LastChange\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name",
            table: "Tags",
            column: "Name");

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
            name: "IX_Tags_Slug",
            table: "Tags",
            column: "Slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Votes_StoryId",
            table: "Votes",
            column: "StoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Votes_UserId_StoryId",
            table: "Votes",
            columns: new[] { "UserId", "StoryId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserPasskeys");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "BlacklistedRatings");

        migrationBuilder.DropTable(
            name: "BlacklistedTags");

        migrationBuilder.DropTable(
            name: "BlockedUsers");

        migrationBuilder.DropTable(
            name: "ChaptersRead");

        migrationBuilder.DropTable(
            name: "ClubMembers");

        migrationBuilder.DropTable(
            name: "ClubModeratorActions");

        migrationBuilder.DropTable(
            name: "CommentRevisions");

        migrationBuilder.DropTable(
            name: "CommentThreadSubscribers");

        migrationBuilder.DropTable(
            name: "Documents");

        migrationBuilder.DropTable(
            name: "Faqs");

        migrationBuilder.DropTable(
            name: "FolderStories");

        migrationBuilder.DropTable(
            name: "FollowedUsers");

        migrationBuilder.DropTable(
            name: "GlobalNotifications");

        migrationBuilder.DropTable(
            name: "Infractions");

        migrationBuilder.DropTable(
            name: "InviteCodes");

        migrationBuilder.DropTable(
            name: "ModeratorActions");

        migrationBuilder.DropTable(
            name: "NotificationRecipients");

        migrationBuilder.DropTable(
            name: "Quotes");

        migrationBuilder.DropTable(
            name: "Reports");

        migrationBuilder.DropTable(
            name: "ShelfStories");

        migrationBuilder.DropTable(
            name: "StoryTags");

        migrationBuilder.DropTable(
            name: "Subscriptions");

        migrationBuilder.DropTable(
            name: "Votes");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "Folders");

        migrationBuilder.DropTable(
            name: "Notifications");

        migrationBuilder.DropTable(
            name: "Comments");

        migrationBuilder.DropTable(
            name: "Shelves");

        migrationBuilder.DropTable(
            name: "Tags");

        migrationBuilder.DropTable(
            name: "SubscriptionTiers");

        migrationBuilder.DropTable(
            name: "CommentThreads");

        migrationBuilder.DropTable(
            name: "Icons");

        migrationBuilder.DropTable(
            name: "TagNamespaces");

        migrationBuilder.DropTable(
            name: "Blogposts");

        migrationBuilder.DropTable(
            name: "ClubThreads");

        migrationBuilder.DropTable(
            name: "News");

        migrationBuilder.DropTable(
            name: "Chapters");

        migrationBuilder.DropTable(
            name: "Clubs");

        migrationBuilder.DropTable(
            name: "Stories");

        migrationBuilder.DropTable(
            name: "ContentBlocks");

        migrationBuilder.DropTable(
            name: "Ratings");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "Images");

        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:CollationDefinition:nocase", "und-u-ks-level2,und-u-ks-level2,icu,False")
            .OldAnnotation("Npgsql:CollationDefinition:nocase-noaccent", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .OldAnnotation("Npgsql:CollationSchemasResolved", true)
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
    }
}
