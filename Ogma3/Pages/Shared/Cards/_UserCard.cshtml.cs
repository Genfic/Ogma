using Ogma3.Data.Roles;

namespace Ogma3.Pages.Shared.Cards;

public sealed class UserCard
{
	public required string UserName { get; init; }
	public required string Avatar { get; init; }
	public required string? Title { get; init; }
	public required IEnumerable<RoleDto> Roles { get; init; } = [];
}