using AutoMapper;
using Ogma3.Data.Blogposts;

namespace Ogma3.Pages.Shared.Cards;

public class BlogpostCard
{
	public required long Id { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required DateTime? PublicationDate { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Body { get; init; }
	public required int WordCount { get; init; }
	public required string[] Hashtags { get; init; }
	
	// TODO: Get rid of Automapper
	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Blogpost, BlogpostCard>();
	}
}