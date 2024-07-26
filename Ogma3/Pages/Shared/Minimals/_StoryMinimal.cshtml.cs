using AutoMapper;
using Ogma3.Data.Stories;

namespace Ogma3.Pages.Shared.Minimals;

public class StoryMinimal
{
	public required long Id { get; init; }
	public required string Title { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Slug { get; init; }
	public required DateTime? PublicationDate { get; init; }

	// TODO: Get rid of Automapper
	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Story, StoryMinimal>();
	}
}