using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Models;

namespace Ogma3.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }


        public DbSet<Tag> Tags { get; set; }
        public DbSet<StoryTag> StoryTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Namespace> Namespaces { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<CommentsThread> CommentThreads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<VotePool> VotePools { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<ShelfStory> ShelfStories { get; set; }
        
        // Secondary
        public DbSet<Document> Documents { get; set; }
        public DbSet<Icon> Icons { get; set; }

        
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // User
            builder.Entity<User>()
                .Ignore(u => u.PhoneNumber)
                .Ignore(u => u.PhoneNumberConfirmed);

            // Tag
            builder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();
            builder.Entity<Tag>()
                .HasOne(t => t.Namespace)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            // Category
            builder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Namespace
            builder.Entity<Namespace>()
                .HasIndex(n => n.Name)
                .IsUnique();
            
            // Rating
            builder.Entity<Rating>()
                .HasIndex(r => r.Name)
                .IsUnique();
            
            // Story
            builder.Entity<Story>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<Story>()
                .Property(p => p.IsPublished)
                .HasDefaultValue(false);
            builder.Entity<Story>()
                .HasOne(s => s.Rating)
                .WithMany();
            builder.Entity<Story>()
                .HasMany(s => s.Chapters)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Story>()
                .HasOne(s => s.Author)
                .WithMany();
            builder.Entity<Story>()
                .HasOne(s => s.VotesPool)
                .WithOne()
                .HasForeignKey<Story>(s => s.VotesPoolId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Chapter
            builder.Entity<Chapter>()
                .Property(p => p.IsPublished)
                .HasDefaultValue(false);
            builder.Entity<Chapter>()
                .HasOne(c => c.CommentsThread)
                .WithOne()
                .HasForeignKey<Chapter>(c => c.CommentsThreadId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .OnDelete(DeleteBehavior.Cascade);

            // Comments
            builder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany();
            
            // Vote pools
            builder.Entity<VotePool>()
                .HasMany(vp => vp.Votes)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            
            // Votes
            builder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany();
            builder.Entity<Vote>()
                .HasIndex(v => new {v.UserId, v.VotePoolId})
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
            

            
            // Documents
            builder.Entity<Document>()
                .HasIndex(d => d.Slug)
                .IsUnique();

        }
        
        
    }
}
