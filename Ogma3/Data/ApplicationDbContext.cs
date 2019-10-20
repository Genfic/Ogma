using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
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
        
        
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
                .Property(p => p.ReleaseDate)
                .HasDefaultValueSql("getdate()");
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
            
            // Chapter
            builder.Entity<Chapter>()
                .Property(c => c.PublishDate)
                .HasDefaultValueSql("getdate()");
            builder.Entity<Chapter>()
                .Property(p => p.IsPublished)
                .HasDefaultValue(false);
                
            // Story tags
            builder.Entity<StoryTag>()
                .HasKey(st => new {st.StoryId, st.TagId});
            builder.Entity<StoryTag>()
                .HasOne(st => st.Story)
                .WithMany(s => s.StoryTags);
            builder.Entity<StoryTag>()
                .HasOne(st => st.Tag)
                .WithMany();

        }
        
        
    }
}
