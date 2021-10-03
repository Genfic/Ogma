using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Converters;

namespace Ogma3.Data.Chapters;

public class ChaptersRead
{
    public Story Story { get; init; }
    public long StoryId { get; init; }
    public OgmaUser User { get; init; }
    public long UserId { get; init; }
    public HashSet<long> Chapters { get; init; }

    public class Configuration : IEntityTypeConfiguration<ChaptersRead>
    {
        public void Configure(EntityTypeBuilder<ChaptersRead> builder)
        {
            builder
                .HasKey(cr => new { cr.StoryId, cr.UserId });

            builder
                .Property(cr => cr.Chapters)
                .HasConversion(new NpgHashsetConverter<long>());
        }
    }
}