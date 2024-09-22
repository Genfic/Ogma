using Ogma3.Data.Ratings;
using Ogma3.Data.Tags;
using Ogma3.Pages.Shared;

namespace Ogma3.Data.Stories;

public sealed class StoryDetails
{
	public required long Id { get; init; }
	public required long AuthorId { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required string Description { get; init; }
	public required string Hook { get; init; }
	public required string Cover { get; init; }
	public required DateTimeOffset ReleaseDate { get; init; }
	public required bool IsPublished { get; init; }
	public required IEnumerable<TagDto> Tags { get; init; }
	public required Rating Rating { get; init; }
	public required EStoryStatus Status { get; init; }
	public required int WordCount { get; init; }
	public required int ChaptersCount { get; init; }
	public required int CommentsCount { get; init; }
	public required int VotesCount { get; init; }
	public required ContentBlockCard? ContentBlock { get; init; }

	public required ICollection<CreditDto> Credits { get; init; }
}

public sealed record CreditDto(string Role, string Name, string? Link);