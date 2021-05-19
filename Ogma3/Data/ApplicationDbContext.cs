using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Documents;
using Ogma3.Data.Faqs;
using Ogma3.Data.Folders;
using Ogma3.Data.Icons;
using Ogma3.Data.InviteCodes;
using Ogma3.Data.ModeratorActions;
using Ogma3.Data.Notifications;
using Ogma3.Data.Quotes;
using Ogma3.Data.Ratings;
using Ogma3.Data.Reports;
using Ogma3.Data.Roles;
using Ogma3.Data.Shelfs;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;
using Ogma3.Data.Votes;
using Serilog;

namespace Ogma3.Data
{
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
        public DbSet<Tag> Tags { get; set; }
        public DbSet<StoryTag> StoryTags { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ChaptersRead> ChaptersRead { get; set; }
        public DbSet<CommentsThread> CommentThreads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentRevision> CommentRevisions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<ShelfStory> ShelfStories { get; set; }
        public DbSet<Blogpost> Blogposts { get; set; }
        public DbSet<UserRole> OgmaUserRoles { get; set; }
        public DbSet<OgmaRole> OgmaRoles { get; set; }
        public DbSet<CommentsThreadSubscriber> CommentsThreadSubscribers { get; set; }

        // Clubs
        public DbSet<Club> Clubs { get; set; }
        public DbSet<ClubMember> ClubMembers { get; set; }
        public DbSet<ClubThread> ClubThreads { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<FolderStory> FolderStories { get; set; }


        // Secondary
        public DbSet<Document> Documents { get; set; }
        public DbSet<Icon> Icons { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        
        // Moderation
        public DbSet<ModeratorAction> ModeratorActions { get; set; }
        public DbSet<ContentBlock> ContentBlocks { get; set; }
        public DbSet<Report> Reports { get; set; }
        
        // Blacklists
        public DbSet<BlacklistedRating> BlacklistedRatings { get; set; }
        public DbSet<BlacklistedTag> BlacklistedTags { get; set; }
        public DbSet<UserBlock> BlacklistedUsers { get; set; }
        
        // Follows
        public DbSet<UserFollow> FollowedUsers { get; set; }
        
        // Notifications
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationRecipients> NotificationRecipients { get; set; }

        // Invite codes
        public DbSet<InviteCode> InviteCodes { get; set; }


        private readonly ILoggerFactory _myLoggerFactory;
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            _myLoggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());
            
            // NOTE: When mapping an enum here, remember to also add it in `OnModelCreating()`
            NpgsqlConnection.GlobalTypeMapper
                .MapEnum<EStoryStatus>()
                .MapEnum<EClubMemberRoles>()
                .MapEnum<EDeletedBy>()
                .MapEnum<ENotificationEvent>()
                .MapEnum<ETagNamespace>();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Extensions
            builder.HasPostgresExtension("uuid-ossp");
            
            // Enums
            // NOTE: When adding an enum here, remember to also map it in `ApplicationDbContext()`
            builder
                .HasPostgresEnum<EStoryStatus>()
                .HasPostgresEnum<EClubMemberRoles>()
                .HasPostgresEnum<EDeletedBy>()
                .HasPostgresEnum<ENotificationEvent>()
                .HasPostgresEnum<ETagNamespace>();

            // Load model configurations
            builder.ApplyConfigurationsFromAssembly(typeof(Startup).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_myLoggerFactory);
        }
    }
}