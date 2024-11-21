using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
	public required DbSet<Tag> Tags { get; init; }
	public required DbSet<StoryTag> StoryTags { get; init; }
	public required DbSet<Story> Stories { get; init; }
	public required DbSet<Rating> Ratings { get; init; }
	public required DbSet<Chapter> Chapters { get; init; }
	public required DbSet<ChaptersRead> ChaptersRead { get; init; }
	public required DbSet<CommentsThread> CommentThreads { get; init; }
	public required DbSet<Comment> Comments { get; init; }
	public required DbSet<CommentRevision> CommentRevisions { get; init; }
	public required DbSet<Vote> Votes { get; init; }
	public required DbSet<Shelf> Shelves { get; init; }
	public required DbSet<ShelfStory> ShelfStories { get; init; }
	public required DbSet<Blogpost> Blogposts { get; init; }
	public required DbSet<UserRole> OgmaUserRoles { get; init; }
	public required DbSet<OgmaRole> OgmaRoles { get; init; }
	public required DbSet<CommentsThreadSubscriber> CommentsThreadSubscribers { get; init; }

	// Clubs
	public required DbSet<Club> Clubs { get; init; }
	public required DbSet<ClubMember> ClubMembers { get; init; }
	public required DbSet<ClubThread> ClubThreads { get; init; }
	public required DbSet<Folder> Folders { get; init; }
	public required DbSet<FolderStory> FolderStories { get; init; }


	// Secondary
	public required DbSet<Document> Documents { get; init; }
	public required DbSet<Icon> Icons { get; init; }
	public required DbSet<Quote> Quotes { get; init; }
	public required DbSet<Faq> Faqs { get; init; }

	// Moderation
	public required DbSet<ModeratorAction> ModeratorActions { get; init; }
	public required DbSet<ClubModeratorAction> ClubModeratorActions { get; init; }
	public required DbSet<ContentBlock> ContentBlocks { get; init; }
	public required DbSet<Report> Reports { get; init; }
	public required DbSet<Infraction> Infractions { get; init; }

	// Blacklists
	public required DbSet<BlacklistedRating> BlacklistedRatings { get; init; }
	public required DbSet<BlacklistedTag> BlacklistedTags { get; init; }
	public required DbSet<UserBlock> BlacklistedUsers { get; init; }

	// Follows
	public required DbSet<UserFollow> FollowedUsers { get; init; }

	// Notifications
	public required DbSet<Notification> Notifications { get; init; }
	public required DbSet<NotificationRecipients> NotificationRecipients { get; init; }

	// Invite codes
	public required DbSet<InviteCode> InviteCodes { get; init; }


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