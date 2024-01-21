using System.Collections.Generic;
using AutoMapper;
using Ogma3.Data.Folders;

namespace Ogma3.Pages.Shared.Cards;

public class FolderCard
{
	public required long Id { get; init; }
	public required long ClubId { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public string? Description { get; init; }

	public required int StoriesCount { get; init; }
	public required IEnumerable<FolderMinimalDto> ChildFolders { get; init; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Folder, FolderCard>();
	}
}