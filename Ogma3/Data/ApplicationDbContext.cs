using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Ogma3.Data.Enums;
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
        public DbSet<ModeratorAction> ModeratorActions { get; set; }
        
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
            builder.Entity<OgmaUser>(ent =>
            {
                ent.Ignore(u => u.PhoneNumber)
                    .Ignore(u => u.PhoneNumberConfirmed);
                ent.HasOne(u => u.CommentsThread)
                    .WithOne(ct => ct.User)
                    .HasForeignKey<CommentsThread>(ct => ct.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                ent.HasMany(u => u.Roles)
                    .WithMany(or => or.Users)
                    .UsingEntity<UserRole>(
                        ur => ur.HasOne(e => e.Role)
                            .WithMany()
                            .HasForeignKey(k => k.RoleId),
                        ur => ur.HasOne(e => e.User)
                            .WithMany(u => u.UserRoles)
                            .HasForeignKey(k => k.UserId)
                    );
                ent.HasMany(u => u.Followers)
                    .WithMany(u => u.Following)
                    .UsingEntity<UserFollow>(
                        uf => uf.HasOne(e => e.FollowingUser)
                            .WithMany()
                            .HasForeignKey(k => k.FollowingUserId),
                        uf => uf.HasOne(e => e.FollowedUser)
                            .WithMany()
                            .HasForeignKey(k => k.FollowedUserId),
                        uf => uf.HasKey(i => new { i.FollowingUserId, i.FollowedUserId })
                    );
                ent.HasMany(u => u.BlockedUsers)
                    .WithMany(u => u.BlockedByUsers)
                    .UsingEntity<UserBlock>(
                        ub => ub.HasOne(e => e.BlockingUser)
                            .WithMany()
                            .HasForeignKey(k => k.BlockingUserId),
                        ub => ub.HasOne(e => e.BlockedUser)
                            .WithMany()
                            .HasForeignKey(k => k.BlockedUserId),
                        ub => ub.HasKey(i => new { i.BlockingUserId, i.BlockedUserId })
                    );
            });

            // Tag
            builder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();
            builder.Entity<Tag>()
                .HasOne(t => t.Namespace)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            // Namespace
            builder.Entity<Namespace>()
                .HasIndex(n => n.Name)
                .IsUnique();

            // Rating
            builder.Entity<Rating>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Story
            builder.Entity<Story>(ent =>
            {
                ent.Property(s => s.Id)
                    .ValueGeneratedOnAdd();
                ent.Property(p => p.IsPublished)
                    .HasDefaultValue(false);
                ent.HasOne(s => s.Rating)
                    .WithMany();
                ent.HasMany(s => s.Chapters)
                    .WithOne(c => c.Story)
                    .OnDelete(DeleteBehavior.Cascade);
                ent.HasOne(s => s.Author)
                    .WithMany(u => u.Stories)
                    .HasForeignKey(s => s.AuthorId);
                ent.HasMany(s => s.Votes)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Chapter
            builder.Entity<Chapter>(ent =>
            {
                ent.Property(p => p.IsPublished)
                    .HasDefaultValue(false);
                ent.HasOne(c => c.CommentsThread)
                    .WithOne(ct => ct.Chapter)
                    .HasForeignKey<CommentsThread>(ct => ct.ChapterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Chapter reads
            builder.Entity<ChaptersRead>(ent =>
            {
                ent.HasOne(cr => cr.Story)
                    .WithMany();
                ent.HasOne(cr => cr.User)
                    .WithMany();
            });

            // Story tags
            builder.Entity<StoryTag>()
                .HasKey(st => new {st.StoryId, st.TagId});
            builder.Entity<StoryTag>()
                .HasOne(st => st.Story)
                .WithMany(s => s.StoryTags);
            builder.Entity<StoryTag>()
                .HasOne(st => st.Tag)
                .WithMany();

            // Comment threads
            builder.Entity<CommentsThread>()
                .HasMany(ct => ct.Comments)
                .WithOne()
                .HasForeignKey(c => c.CommentsThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comments
            builder.Entity<Comment>(ent =>
            {
                ent.HasOne(c => c.Author)
                    .WithMany();
                ent.HasOne(c => c.DeletedByUser)
                    .WithMany();
            });
            
            // Comment revisions
            builder.Entity<CommentRevision>(ent =>
            {
                ent.HasOne(cr => cr.Parent)
                    .WithMany(c => c.Revisions)
                    .HasForeignKey(cr => cr.ParentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // Votes
            builder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany();
            builder.Entity<Vote>()
                .HasIndex(v => new {v.UserId, v.StoryId})
                .IsUnique();

            // Shelf stories
            builder.Entity<ShelfStory>()
                .HasKey(ss => new {ss.ShelfId, ss.StoryId});
            builder.Entity<ShelfStory>()
                .HasOne(ss => ss.Shelf)
                .WithMany(s => s.ShelfStories)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ShelfStory>()
                .HasOne(ss => ss.Story)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            // Shelves
            builder.Entity<Shelf>()
                .HasOne(s => s.Icon)
                .WithMany();

            // Blogposts
            builder.Entity<Blogpost>(ent =>
            {
                ent.HasOne(b => b.Author)
                    .WithMany(u => u.Blogposts)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
                ent.HasOne(b => b.CommentsThread)
                    .WithOne(ct => ct.Blogpost)
                    .HasForeignKey<CommentsThread>(ct => ct.BlogpostId)
                    .OnDelete(DeleteBehavior.Cascade);
                ent.HasOne(b => b.AttachedStory)
                    .WithMany()
                    .HasForeignKey(b => b.AttachedStoryId)
                    .OnDelete(DeleteBehavior.SetNull);
                ent.HasOne(b => b.AttachedChapter)
                    .WithMany()
                    .HasForeignKey(b => b.AttachedChapterId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
                

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