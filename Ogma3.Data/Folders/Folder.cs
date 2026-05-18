using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Clubs;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Folders;

[AutoDbSet]
public sealed class Folder : BaseModel
{
	public required string Name { get; set; }
	public required string Slug { get; set; }
	public string? Description { get; set; }

	public Club Club { get; set; } = null!;
	public required long ClubId { get; set; }
	public List<Story> Stories { get; init; } = [];
	public EClubMemberRoles AccessLevel { get; set; }
}