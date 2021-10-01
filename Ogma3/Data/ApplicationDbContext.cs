using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<StoryTag> StoryTags { get; set; } = null!;
    public DbSet<Story> Stories { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<Chapter> Chapters { get; set; } = null!;
    public DbSet<ChaptersRead> ChaptersRead { get; set; } = null!;
    public DbSet<CommentsThread> CommentThreads { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<CommentRevision> CommentRevisions { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;
    public DbSet<Shelf> Shelves { get; set; } = null!;
    public DbSet<ShelfStory> ShelfStories { get; set; } = null!;
    public DbSet<Blogpost> Blogposts { get; set; } = null!;
    public DbSet<UserRole> OgmaUserRoles { get; set; } = null!;
    public DbSet<OgmaRole> OgmaRoles { get; set; } = null!;
    public DbSet<CommentsThreadSubscriber> CommentsThreadSubscribers { get; set; } = null!;

    // Clubs
    public DbSet<Club> Clubs { get; set; } = null!;
    public DbSet<ClubMember> ClubMembers { get; set; } = null!;
    public DbSet<ClubThread> ClubThreads { get; set; } = null!;
    public DbSet<Folder> Folders { get; set; } = null!;
    public DbSet<FolderStory> FolderStories { get; set; } = null!;


    // Secondary
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<Icon> Icons { get; set; } = null!;
    public DbSet<Quote> Quotes { get; set; } = null!;
    public DbSet<Faq> Faqs { get; set; } = null!;
        
    // Moderation
    public DbSet<ModeratorAction> ModeratorActions { get; set; } = null!;
    public DbSet<ClubModeratorAction> ClubModeratorActions { get; set; } = null!;
    public DbSet<ContentBlock> ContentBlocks { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<Infraction> Infractions { get; set; } = null!;
        
    // Blacklists
    public DbSet<BlacklistedRating> BlacklistedRatings { get; set; } = null!;
    public DbSet<BlacklistedTag> BlacklistedTags { get; set; } = null!;
    public DbSet<UserBlock> BlacklistedUsers { get; set; } = null!;
        
    // Follows
    public DbSet<UserFollow> FollowedUsers { get; set; } = null!;
        
    // Notifications
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationRecipients> NotificationRecipients { get; set; } = null!;

    // Invite codes
    public DbSet<InviteCode> InviteCodes { get; set; } = null!;


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