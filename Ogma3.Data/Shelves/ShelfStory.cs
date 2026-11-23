using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Shelves;

[AutoDbSet]
public sealed class ShelfStory
{
	public Shelf Shelf { get; init; } = null!;
	public long ShelfId { get; init; }
	public Story Story { get; init; } = null!;
	public long StoryId { get; init; }

	public sealed class Configuration : IEntityTypeConfiguration<ShelfStory>
	{
		public void Configure(EntityTypeBuilder<ShelfStory> builder)
		{
			// Matches the query filter on Story, so rows referencing soft-deleted stories stay hidden
			builder.HasQueryFilter(ss => ss.Story.ScheduledForDeletion == null);
		}
	}
}
