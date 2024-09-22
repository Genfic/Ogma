using Ogma3.Data.Clubs;

namespace Ogma3.Pages.Shared.Bars;

public sealed class ClubBar
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public required string Hook { get; init; }
	public required string Description { get; init; }
	public required string Icon { get; init; }
	public required DateTimeOffset CreationDate { get; init; }
	public required int ClubMembersCount { get; init; }
	public required int ThreadsCount { get; init; }
	public required int StoriesCount { get; init; }
	public bool IsMember => Role is not null;
	public required long FounderId { get; init; }
	public required EClubMemberRoles? Role { get; init; }
}