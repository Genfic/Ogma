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
            builder.Entity<Club>(ent =>
            {
                ent.HasMany(c => c.Threads)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
                ent.HasMany(c => c.Folders)
                    .WithOne(f => f.Club)
                    .HasForeignKey(f => f.ClubId)
                    .OnDelete(DeleteBehavior.Cascade);
                ent.HasMany(c => c.Reports)
                    .WithOne(r => r.Club)
                    .HasForeignKey(r => r.ClubId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Club members
            builder.Entity<ClubMember>(ent =>
            {
                ent.HasKey(cm => new {cm.ClubId, cm.MemberId});
                ent.HasOne(cm => cm.Club)
                    .WithMany(c => c.ClubMembers)
                    .OnDelete(DeleteBehavior.Cascade);
                ent.HasOne(cm => cm.Member)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Club threads
            builder.Entity<ClubThread>(ent =>
            {
                ent.HasOne(ct => ct.Author)
                    .WithMany()
                    .HasForeignKey(ct => ct.AuthorId)
                    .OnDelete(DeleteBehavior.SetNull);
                ent.HasOne(b => b.CommentsThread)
                    .WithOne(ct => ct.ClubThread)
                    .HasForeignKey<CommentsThread>(ct => ct.ClubThreadId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Folders
            builder.Entity<Folder>(ent =>
            {
                ent.HasMany(f => f.ChildFolders)
                    .WithOne(f => f.ParentFolder)
                    .HasForeignKey(f => f.ParentFolderId)
                    .OnDelete(DeleteBehavior.Cascade);
                ent.Property(f => f.AccessLevel)
                    .HasDefaultValue(EClubMemberRoles.User);
                ent.HasMany(f => f.Stories)
                    .WithMany(s => s.Folders)
                    .UsingEntity<FolderStory>(
                        fs => fs.HasOne(f => f.Story)
                            .WithMany()
                            .HasForeignKey(f => f.StoryId),
                        fs => fs.HasOne(f => f.Folder)
                            .WithMany()
                            .HasForeignKey(f => f.FolderId)
                        );
            });
            
            
            // Blacklisted ratings
            builder.Entity<BlacklistedRating>(ent =>
            {
                ent.HasKey(br => new { br.UserId, br.RatingId });
                ent.HasOne(e => e.Rating)
                    .WithMany();
                ent.HasOne(e => e.User)
                    .WithMany(u => u.BlacklistedRatings);
            });
            
            // Blacklisted tags
            builder.Entity<BlacklistedTag>(ent =>
            {
                ent.HasKey(bt => new { bt.UserId, bt.TagId });
                ent.HasOne(e => e.Tag)
                    .WithMany();
                ent.HasOne(e => e.User)
                    .WithMany(u => u.BlacklistedTags);
            });

            
            
            // Enums
            builder.HasPostgresEnum<EStoryStatus>();
            builder.HasPostgresEnum<EClubMemberRoles>();
            builder.HasPostgresEnum<EDeletedBy>();


            // Documents
            builder.Entity<Document>(ent =>
            {
                ent.HasIndex(d => new { d.Slug, d.Version })
                    .IsUnique();
                ent.HasIndex(d => new { d.Title, d.Version })
                    .IsUnique();
            });

            // Invite codes
            builder.Entity<InviteCode>(ent =>
            {
                ent.HasOne(c => c.UsedBy)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
                ent.HasOne(c => c.IssuedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Moderator actions
            builder.Entity<ModeratorAction>(ent =>
            {
                ent.HasOne(ma => ma.StaffMember)
                    .WithMany()
                    .HasForeignKey(ma => ma.StaffMemberId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}