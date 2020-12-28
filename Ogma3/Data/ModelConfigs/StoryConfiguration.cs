using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Infrastructure;

namespace Ogma3.Data.ModelConfigs
{
    public class StoryConfiguration : BaseConfiguration<Story>
    {
        public override void Configure(EntityTypeBuilder<Story> builder)
        {
            base.Configure(builder);

            // CONSTRAINTS
            builder
                .Property(p => p.IsPublished)
                .HasDefaultValue(false);

            builder
                .Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(CTConfig.CStory.MaxTitleLength);

            builder
                .Property(s => s.Slug)
                .IsRequired()
                .HasMaxLength(CTConfig.CStory.MaxTitleLength);

            builder
                .Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(CTConfig.CStory.MaxDescriptionLength);
            
            builder
                .Property(s => s.Hook)
                .IsRequired()
                .HasMaxLength(CTConfig.CStory.MaxHookLength);

            builder
                .Property(s => s.ReleaseDate)
                .IsRequired()
                .HasDefaultValueSql(PgConstants.CurrentTimestamp);

            builder
                .Property(s => s.IsPublished)
                .IsRequired()
                .HasDefaultValue(false);

            builder
                .Property(s => s.Status)
                .IsRequired()
                .HasDefaultValue(EStoryStatus.InProgress);

            builder
                .Property(s => s.WordCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder
                .Property(s => s.ChapterCount)
                .IsRequired()
                .HasDefaultValue(0);
            

            // NAVIGATION
            builder
                .HasOne(s => s.Rating)
                .WithMany()
                .HasForeignKey(s => s.RatingId);
            
            builder
                .HasMany(s => s.Chapters)
                .WithOne(c => c.Story)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(s => s.Tags)
                .WithMany(t => t.Stories)
                .UsingEntity<StoryTag>(
                    st => st.HasOne(e => e.Tag)
                        .WithMany()
                        .HasForeignKey(e => e.TagId)
                        .OnDelete(DeleteBehavior.Cascade),
                    st => st.HasOne(e => e.Story)
                        .WithMany()
                        .HasForeignKey(e => e.StoryId)
                        .OnDelete(DeleteBehavior.Cascade)
                );
            
            builder
                .HasOne(s => s.Author)
                .WithMany(u => u.Stories)
                .HasForeignKey(s => s.AuthorId);
            
            builder
                .HasMany(s => s.Votes)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(s => s.ContentBlock)
                .WithOne()
                .HasForeignKey<Story>(s => s.ContentBlockId)
                .IsRequired(false);
            
            builder
                .HasMany(s => s.Reports)
                .WithOne(r => r.Story)
                .HasForeignKey(r => r.StoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}