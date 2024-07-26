using Ogma3.Data.Roles;

namespace Ogma3.Pages.Shared.Bars;

public class ProfileBar
{
	public required long Id { get; init; }
	public required string UserName { get; init; }
	public required string? Title { get; init; }
	public required string Avatar { get; init; }
	public required string Email { get; init; }
	public required DateTime RegistrationDate { get; init; }
	public required DateTime LastActive { get; init; }
	public required IEnumerable<RoleDto> Roles { get; init; }
	public required int StoriesCount { get; init; }
	public required int BlogpostsCount { get; init; }
	public required int FollowersCount { get; init; }
	public required bool IsBlockedBy { get; init; }
	public required bool IsFollowedBy { get; init; }
}