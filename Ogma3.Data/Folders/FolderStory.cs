using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Folders;

[AutoDbSet]
public sealed class FolderStory
{
	public Folder Folder { get; set; } = null!;
	public long FolderId { get; init; }
	public Story Story { get; set; } = null!;
	public long StoryId { get; init; }
	public DateTimeOffset Added { get; init; }
	public OgmaUser AddedBy { get; set; } = null!;
	public long AddedById { get; init; }

	public sealed class Configuration : IEntityTypeConfiguration<FolderStory>
	{
		public void Configure(EntityTypeBuilder<FolderStory> builder)
		{
			// Matches the query filter on Story, so rows referencing soft-deleted stories stay hidden
			builder.HasQueryFilter(fs => fs.Story.ScheduledForDeletion == null);
		}
	}
}
