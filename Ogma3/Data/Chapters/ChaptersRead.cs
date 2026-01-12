using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Chapters;

[AutoDbSet(Name = nameof(ChaptersRead))]
public sealed class ChaptersRead
{
	public Story Story { get; init; } = null!;
	public long StoryId { get; init; }
	public OgmaUser User { get; init; } = null!;
	public long UserId { get; init; }

	// TODO: Workaround for #82
	private List<long> _chaptersInternal = [];
	public IReadOnlySet<long> Chapters => _chaptersInternal.ToHashSet();

	public void AddChapter(long chapterId)
	{
		if (_chaptersInternal.Contains(chapterId)) return;
		_chaptersInternal.Add(chapterId);
	}

	public void RemoveChapter(long chapterId)
	{
		_chaptersInternal.Remove(chapterId);
	}
	// END WORKAROUND

	public sealed class Configuration : IEntityTypeConfiguration<ChaptersRead>
	{
		public void Configure(EntityTypeBuilder<ChaptersRead> builder)
		{
			builder
				.HasKey(cr => new { cr.StoryId, cr.UserId });

			builder
				.PrimitiveCollection(nameof(_chaptersInternal))
				.HasColumnName(nameof(Chapters));

			builder
				.Ignore(cr => cr.Chapters);
		}
	}
}