using AutoMapper;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;

namespace Ogma3.Pages.Shared.Cards;

public class StoryCard
{
	public required long Id { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required string Hook { get; init; }
	public required string Cover { get; init; }
	public required string? CoverId { get; init; }
	public required DateTime? PublicationDate { get; init; }
	public required IList<TagDto> Tags { get; init; }
	public required Rating Rating { get; init; }
	public required EStoryStatus Status { get; init; }
	public required int WordCount { get; init; }
	public required int ChapterCount { get; init; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Story, StoryCard>()
			.ForMember(sc => sc.Tags, opts
				=> opts.MapFrom(s => s.Tags.OrderByDescending(t => t.Namespace.HasValue).ThenByDescending(t => t.Namespace))
			);
	}
}