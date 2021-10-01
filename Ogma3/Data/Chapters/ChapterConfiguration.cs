using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Chapters;

public class ChapterConfiguration : BaseConfiguration<Chapter>
{
    public override void Configure(EntityTypeBuilder<Chapter> builder)
    {
        base.Configure(builder);
            
        // CONSTRAINTS
        builder
            .Property(c => c.PublicationDate)
            .HasDefaultValue(null);
            
        builder
            .Property(p => p.CreationDate)
            .IsRequired()
            .HasDefaultValueSql(PgConstants.CurrentTimestamp);

        builder
            .Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(CTConfig.CChapter.MaxTitleLength);
            
        builder
            .Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(CTConfig.CChapter.MaxTitleLength);

        builder
            .Property(c => c.Body)
            .IsRequired()
            .HasMaxLength(CTConfig.CChapter.MaxBodyLength);

        builder
            .Property(c => c.StartNotes)
            .IsRequired(false)
            .HasMaxLength(CTConfig.CChapter.MaxNotesLength);

        builder
            .Property(c => c.EndNotes)
            .IsRequired(false)
            .HasMaxLength(CTConfig.CChapter.MaxNotesLength);

        // NAVIGATION
        builder
            .HasOne(c => c.CommentsThread)
            .WithOne(ct => ct.Chapter)
            .HasForeignKey<CommentsThread>(ct => ct.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder
            .HasOne(p => p.ContentBlock)
            .WithOne()
            .HasForeignKey<Chapter>(c => c.ContentBlockId)
            .IsRequired(false);
            
        builder
            .HasMany(c => c.Reports)
            .WithOne(r => r.Chapter)
            .HasForeignKey(r => r.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}