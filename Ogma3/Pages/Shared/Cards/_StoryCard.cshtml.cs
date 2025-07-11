using MemoryPack;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;

namespace Ogma3.Pages.Shared.Cards;

[MemoryPackable]
public sealed partial class StoryCard
{
	public required long Id { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required string Hook { get; init; }
	public required string? Cover { get; init; }
	public required DateTimeOffset? PublicationDate { get; init; }
	public required IEnumerable<TagDto> Tags { get; init; }
	public required RatingIcon Rating { get; init; }
	public required EStoryStatus Status { get; init; }
	public required int WordCount { get; init; }
	public required int ChapterCount { get; init; }
}