using Ogma3.Data.Roles;

namespace Ogma3.Data.Users;

public class UserSimpleDto
{
	public required string UserName { get; init; }
	public required string Avatar { get; init; }
	public string? Title { get; init; }
	public IEnumerable<RoleDto> Roles { get; init; } = [];
}