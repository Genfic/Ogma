using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;

namespace Ogma3.Areas.Admin.Pages;

public sealed class Users(ApplicationDbContext context) : PageModel
{
	public required UserDetailsDto? OgmaUser { get; set; }
	public required List<RoleDto> Roles { get; set; }
	public string InfractionNamesJson =>
		JsonSerializer.Serialize(InfractionTypeExtensions.GetNames(), InfractionNamesJsonContext.Default.StringArray);
	public string RolesJson =>
		JsonSerializer.Serialize(Roles, RoleDtoJsonContext.Default.ListRoleDto);

	public string? Name { get; set;  }

	public async Task<ActionResult> OnGet([FromQuery] string? name)
	{
		Name = name;

		Roles = await context.Roles
			.Select(r => new RoleDto(r.Id, r.Name))
			.ToListAsync();

		return Page();
	}

	public sealed record UserDetailsDto
	{
		public required long Id { get; init; }
		public required string Name { get; init; } = null!;
		public required string Email { get; init; } = null!;
		public required string? Title { get; init; }
		public required string? Avatar { get; init; }
		public required DateTimeOffset RegistrationDate { get; init; }
		public required DateTimeOffset LastActive { get; init; }
		public required int StoriesCount { get; init; }
		public required int BlogpostsCount { get; init; }
		public required IEnumerable<string> RoleNames { get; init; } = null!;
		public required ICollection<InfractionDto> Infractions { get; init; } = null!;
	}

	public sealed record InfractionDto
	{
		public required long Id { get; init; }
		public required DateTimeOffset IssueDate { get; init; }
		public required DateTimeOffset ActiveUntil { get; init; }
		public required DateTimeOffset? RemovedAt { get; init; }
		public required string Reason { get; init; }
		public required InfractionType Type { get; init; }
		public required string? RemovedBy { get; init; }
	}

	public sealed record RoleDto(long Id, string Name);
}

[JsonSerializable(typeof(string[]))]
public sealed partial class InfractionNamesJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(List<Users.RoleDto>))]
public sealed partial class RoleDtoJsonContext : JsonSerializerContext;