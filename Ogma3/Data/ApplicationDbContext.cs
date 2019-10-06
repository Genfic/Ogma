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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Namespace> Namespaces { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        
        
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tag
            builder.Entity<Tag>()
                .HasAlternateKey(c => c.Name);
            builder.Entity<Tag>()
                .HasOne(t => t.Namespace)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            // Category
            builder.Entity<Category>()
                .HasAlternateKey(c => c.Name);

            // Namespace
            builder.Entity<Namespace>()
                .HasAlternateKey(n => n.Name);
            
            // Rating
            builder.Entity<Rating>()
                .HasAlternateKey(r => r.Name);
            
            // Story
            builder.Entity<Story>()
                .Property(p => p.ReleaseDate)
                .HasDefaultValue(DateTime.Now);
            builder.Entity<Story>()
                .HasOne(s => s.Rating)
                .WithMany();
            
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
