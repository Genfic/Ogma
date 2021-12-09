using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ogma3.Areas.Admin.Models;
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
using Ogma3.Infrastructure.PostgresEnumHelper;
using Serilog;

namespace Ogma3.Data;

public class ApplicationDbContext : IdentityDbContext
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
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<StoryTag> StoryTags => Set<StoryTag>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<ChaptersRead> ChaptersRead => Set<ChaptersRead>();
    public DbSet<CommentsThread> CommentThreads => Set<CommentsThread>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<CommentRevision> CommentRevisions => Set<CommentRevision>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Shelf> Shelves => Set<Shelf>();
    public DbSet<ShelfStory> ShelfStories => Set<ShelfStory>();
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
    public DbSet<UserRole> OgmaUserRoles => Set<UserRole>();
    public DbSet<OgmaRole> OgmaRoles => Set<OgmaRole>();
    public DbSet<CommentsThreadSubscriber> CommentsThreadSubscribers => Set<CommentsThreadSubscriber>();

    // Clubs
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<ClubMember> ClubMembers => Set<ClubMember>();
    public DbSet<ClubThread> ClubThreads => Set<ClubThread>();
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<FolderStory> FolderStories => Set<FolderStory>();


    // Secondary
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Icon> Icons => Set<Icon>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<Faq> Faqs => Set<Faq>();
        
    // Moderation
    public DbSet<ModeratorAction> ModeratorActions => Set<ModeratorAction>();
    public DbSet<ClubModeratorAction> ClubModeratorActions => Set<ClubModeratorAction>();
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Infraction> Infractions => Set<Infraction>();
        
    // Blacklists
    public DbSet<BlacklistedRating> BlacklistedRatings => Set<BlacklistedRating>();
    public DbSet<BlacklistedTag> BlacklistedTags => Set<BlacklistedTag>();
    public DbSet<UserBlock> BlacklistedUsers => Set<UserBlock>();
        
    // Follows
    public DbSet<UserFollow> FollowedUsers => Set<UserFollow>();
        
    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipients> NotificationRecipients => Set<NotificationRecipients>();

    // Invite codes
    public DbSet<InviteCode> InviteCodes => Set<InviteCode>();
    
    // Keyless
    public DbSet<TableInfo> TableInfos => Set<TableInfo>();
    public DbSet<TableRowCount> TableRowCounts => Set<TableRowCount>();
    

    private readonly ILoggerFactory _myLoggerFactory;
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        _myLoggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());
            
        // Map all enums with `[PostgresEnum]` attribute
        NpgsqlConnection.GlobalTypeMapper.MapPostgresEnums(typeof(Startup).Assembly);
    }
        
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Extensions
        builder.HasPostgresExtension("uuid-ossp");
            
        // Register all enums with `[PostgresEnum]` attribute
        builder.RegisterPostgresEnums(typeof(Startup).Assembly);

        // Load model configurations
        builder.ApplyConfigurationsFromAssembly(typeof(Startup).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLoggerFactory(_myLoggerFactory);
    }
}