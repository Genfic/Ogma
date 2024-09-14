#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Chapters;

public sealed class ChaptersRead
{
	public Story Story { get; init; }
	public long StoryId { get; init; }
	public OgmaUser User { get; init; }
	public long UserId { get; init; }
	// TODO: Should be a HashSet<long>, but #82 blocks that
	public List<long> Chapters { get; set; }

	public sealed class Configuration : IEntityTypeConfiguration<ChaptersRead>
	{
		public void Configure(EntityTypeBuilder<ChaptersRead> builder)
		{
			builder
				.HasKey(cr => new { cr.StoryId, cr.UserId });

			builder
				.PrimitiveCollection(cr => cr.Chapters);
			// .HasConversion(
			// 	v => v.ToList(), 
			// 	v => v.ToHashSet()) // BUG: This bitch broken
			// .Metadata.SetValueComparer(new ValueComparer<HashSet<long>>(
			// 	(a, b) => a.SetEquals(b),
			// 	l => l.Aggregate(0, (i, l1) => HashCode.Combine(i, l1.GetHashCode())),
			// 	h => h.ToHashSet()
			// ));
		}
	}
}