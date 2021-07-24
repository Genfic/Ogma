using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Bases;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Chapters
{
    public class ChaptersRead : BaseModel
    {
        public Story Story { get; init; }
        public long StoryId { get; init; }
        public OgmaUser User { get; init; }
        public long UserId { get; init; }
        public HashSet<long> Chapters { get; init; }

        public class Configuration : BaseConfiguration<ChaptersRead>
        {
            public override void Configure(EntityTypeBuilder<ChaptersRead> builder)
            {
                base.Configure(builder);
                builder.Property(cr => cr.Chapters)
                    .HasConversion(
                        v => new List<long>(v),
                        v => new HashSet<long>(v)
                    );
            }
        }
    }
}