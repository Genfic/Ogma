using AutoMapper;
using Ogma3.Data.Chapters;

namespace Ogma3.Pages.Shared.Minimals;

public class ChapterMinimal
{
	public required long Id { get; init; }
	public required string StoryTitle { get; init; }
	public required string StoryAuthorUserName { get; init; }
	public required long StoryId { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required DateTime? PublicationDate { get; init; }

	// TODO: Get rid of Automapper
	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Chapter, ChapterMinimal>();
	}
}