using System;
using System.Collections.Generic;
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
        
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tag
            builder.Entity<Tag>()
                .HasAlternateKey(c => c.Name);
            builder.Entity<Tag>()
                .HasOne(t => t.Namespace)
                .WithMany(n => n.Tags);

            // Category
            builder.Entity<Category>()
                .HasAlternateKey(c => c.Name);

            // Namespace
            builder.Entity<Namespace>()
                .HasAlternateKey(n => n.Name);
            builder.Entity<Namespace>()
                .HasMany(n => n.Tags)
                .WithOne(t => t.Namespace)
                .OnDelete(DeleteBehavior.SetNull);
        }
        
        
    }
}
