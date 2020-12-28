using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Ogma3.Data.Enums;
using Ogma3.Data.ModelConfigs;
using Ogma3.Data.Models;

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
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            NpgsqlConnection.GlobalTypeMapper
                .MapEnum<EStoryStatus>()
                .MapEnum<EClubMemberRoles>();
        }


        public DbSet<Tag> Tags { get; set; }
        public DbSet<StoryTag> StoryTags { get; set; }
        public DbSet<Namespace> Namespaces { get; set; }
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

        // Invite codes
        public DbSet<InviteCode> InviteCodes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Extensions
            builder.HasPostgresExtension("uuid-ossp");
            
            
            // Enums
            builder.HasPostgresEnum<EStoryStatus>();
            builder.HasPostgresEnum<EClubMemberRoles>();
            builder.HasPostgresEnum<EDeletedBy>();
            
            
            // User
            builder.ApplyConfiguration(new OgmaUserConfiguration());
            // Tag
            builder.ApplyConfiguration(new TagConfiguration());
            // Namespace
            builder.ApplyConfiguration(new NamespaceConfiguration());
            // Rating
            builder.ApplyConfiguration(new RatingConfiguration());
            // Story
            builder.ApplyConfiguration(new StoryConfiguration());
            // Chapter
            builder.ApplyConfiguration(new ChapterConfiguration());
            
            // Comment threads
            builder.Entity<CommentsThread>()
                .HasMany(ct => ct.Comments)
                .WithOne()
                .HasForeignKey(c => c.CommentsThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comments
            builder.ApplyConfiguration(new CommentConfiguration());
            // Comment revisions
            builder.ApplyConfiguration(new CommentRevisionConfiguration());
            // Votes
            builder.ApplyConfiguration(new VoteConfiguration());
            // Shelves
            builder.ApplyConfiguration(new ShelfConfiguration());
            // Blogposts
            builder.ApplyConfiguration(new BlogpostConfiguration());
            // Clubs
            builder.ApplyConfiguration(new ClubConfiguration());
            // Club members
            builder.ApplyConfiguration(new ClubMemberConfiguration());
            // Club threads
            builder.ApplyConfiguration(new ClubThreadConfiguration());
            // Folders
            builder.ApplyConfiguration(new FolderConfiguration());
            // Blacklisted ratings
            builder.ApplyConfiguration(new BlacklistedRatingConfiguration());
            // Blacklisted tags
            builder.ApplyConfiguration(new BlacklistedTagConfiguration());
            
            
            // Documents
            builder.ApplyConfiguration(new DocumentConfiguration());
            // Invite codes
            builder.ApplyConfiguration(new InviteCodeConfiguration());
            // Moderator actions
            builder.ApplyConfiguration(new ModeratorActionConfiguration());

        }
    }
}