using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NpgSqlGenerators;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.Clubs;
using Ogma3.Data.ClubThreads;
using Ogma3.Data.Comments;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Documents;
using Ogma3.Data.Faqs;
using Ogma3.Data.Folders;
using Ogma3.Data.Icons;
using Ogma3.Data.Infractions;
using Ogma3.Data.InviteCodes;
using Ogma3.Data.ModeratorActions;
using Ogma3.Data.Notifications;
using Ogma3.Data.Quotes;
using Ogma3.Data.Ratings;
using Ogma3.Data.Reports;
using Ogma3.Data.Roles;
using Ogma3.Data.Shelves;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;
using Ogma3.Data.Votes;
using Serilog;

namespace Ogma3.Data;

public sealed class ApplicationDbContext : IdentityDbContext
<
	OgmaUser,
	OgmaRole,
	long,
	IdentityUserClaim<long>,
	UserRole,
	IdentityUserLogin<long>,
	IdentityRoleClaim<long>,
	IdentityUserToken<long>
>
{
	public required DbSet<Tag> Tags { get; set; }
	public required DbSet<StoryTag> StoryTags { get; set; }
	public required DbSet<Story> Stories { get; set; }
	public required DbSet<Rating> Ratings { get; set; }
	public required DbSet<Chapter> Chapters { get; set; }
	public required DbSet<ChaptersRead> ChaptersRead { get; set; }
	public required DbSet<CommentsThread> CommentThreads { get; set; }
	public required DbSet<Comment> Comments { get; set; }
	public required DbSet<CommentRevision> CommentRevisions { get; set; }
	public required DbSet<Vote> Votes { get; set; }
	public required DbSet<Shelf> Shelves { get; set; }
	public required DbSet<ShelfStory> ShelfStories { get; set; }
	public required DbSet<Blogpost> Blogposts { get; set; }
	public required DbSet<UserRole> OgmaUserRoles { get; set; }
	public required DbSet<OgmaRole> OgmaRoles { get; set; }
	public required DbSet<CommentsThreadSubscriber> CommentsThreadSubscribers { get; set; }

	// Clubs
	public required DbSet<Club> Clubs { get; set; }
	public required DbSet<ClubMember> ClubMembers { get; set; }
	public required DbSet<ClubThread> ClubThreads { get; set; }
	public required DbSet<Folder> Folders { get; set; }
	public required DbSet<FolderStory> FolderStories { get; set; }
	public required DbSet<ClubBan> ClubBans { get; set; }


	// Secondary
	public required DbSet<Document> Documents { get; set; }
	public required DbSet<Icon> Icons { get; set; }
	public required DbSet<Quote> Quotes { get; set; }
	public required DbSet<Faq> Faqs { get; set; }

	// Moderation
	public required DbSet<ModeratorAction> ModeratorActions { get; set; }
	public required DbSet<ClubModeratorAction> ClubModeratorActions { get; set; }
	public required DbSet<ContentBlock> ContentBlocks { get; set; }
	public required DbSet<Report> Reports { get; set; }
	public required DbSet<Infraction> Infractions { get; set; }

	// Blacklists
	public required DbSet<BlacklistedRating> BlacklistedRatings { get; set; }
	public required DbSet<BlacklistedTag> BlacklistedTags { get; set; }
	public required DbSet<UserBlock> BlacklistedUsers { get; set; }

	// Follows
	public required DbSet<UserFollow> FollowedUsers { get; set; }

	// Notifications
	public required DbSet<Notification> Notifications { get; set; }
	public required DbSet<NotificationRecipients> NotificationRecipients { get; set; }

	// Invite codes
	public required DbSet<InviteCode> InviteCodes { get; set; }


	private readonly ILoggerFactory _myLoggerFactory;

	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
		_myLoggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Extensions
		builder
			.HasPostgresExtension("uuid-ossp")
			.HasPostgresExtension("tsm_system_rows");

		// Register all enums with `[PostgresEnum]` attribute
		builder.RegisterPostgresEnums();

		// Load model configurations
		builder.ApplyConfigurationsFromAssembly(typeof(Startup).Assembly);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder
			.UseLoggerFactory(_myLoggerFactory);
	}
}