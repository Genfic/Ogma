using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Tags;

namespace Ogma3.Data.Stories;

[AutoDbSet]
public sealed class StoryTag
{
	public Story Story { get; set; } = null!;
	public long StoryId { get; set; }
	public Tag Tag { get; set; } = null!;
	public long TagId { get; set; }

	public sealed class Configuration : IEntityTypeConfiguration<StoryTag>
	{
		public void Configure(EntityTypeBuilder<StoryTag> builder)
		{
			// Matches the query filter on Story, so rows referencing soft-deleted stories stay hidden
			builder.HasQueryFilter(st => st.Story.ScheduledForDeletion == null);
		}
	}
}
