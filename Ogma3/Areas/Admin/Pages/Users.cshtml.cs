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

	public async Task<ActionResult> OnGet([FromQuery] string? name)
	{
		if (string.IsNullOrEmpty(name)) return Page();

		var query = context.Users.AsQueryable();

		if (name.StartsWith("id:", StringComparison.InvariantCultureIgnoreCase) && int.TryParse(name[3..], out var id))
		{
			query = query.Where(u => u.Id == id);
		}
		else
		{
			query = query.Where(u => u.NormalizedUserName == name.ToUpperInvariant());
		}

		OgmaUser = await query.Select(u => new UserDetailsDto
			{
				Id = u.Id,
				Name = u.UserName,
				Email = u.Email,
				Title = u.Title,
				Avatar = u.Avatar,
				RoleNames = u.Roles.Select(r => r.Name),
				RegistrationDate = u.RegistrationDate,
				LastActive = u.LastActive,
				StoriesCount = u.Stories.Count,
				BlogpostsCount = u.Blogposts.Count,
				Infractions = u.Infractions
					.OrderByDescending(i => i.Type)
					.ThenByDescending(i => i.ActiveUntil)
					.Select(i => new InfractionDto
					{
						Id = i.Id,
						Reason = i.Reason,
						Type = i.Type,
						ActiveUntil = i.ActiveUntil,
						IssueDate = i.IssueDate,
						RemovedAt = i.RemovedAt,
						RemovedBy = i.RemovedBy != null ? i.RemovedBy.UserName : null,
					})
					.ToList(),
			})
			.FirstOrDefaultAsync();

		if (OgmaUser is null) return NotFound();

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