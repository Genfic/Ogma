using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.UserService;

namespace Ogma3.Areas.Admin.Pages.Users;

[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed class Index
(
	ApplicationDbContext context,
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager,
	IUserService userService,
	ILogger<Index> logger
) : PageModel
{
	public required UserDetailsDto? OgmaUser { get; set; }
	public required List<RoleDto> Roles { get; set; }
	public string InfractionNamesJson
		=> JsonSerializer.Serialize(InfractionTypeExtensions.GetNames(), InfractionNamesJsonContext.Default.StringArray);
	public string RolesJson => JsonSerializer.Serialize(Roles, RoleDtoJsonContext.Default.ListRoleDto);

	public string? Name { get; set; }

	public async Task<ActionResult> OnGet([FromQuery] string? name = null)
	{
		if (name is not null)
		{
			Name = await context.Users
				.Where(u => EF.Functions.ILike(u.UserName, $"%{name}%"))
				.Select(u => u.UserName)
				.FirstOrDefaultAsync();
		}

		Roles = await context.Roles
			.Select(r => new RoleDto(r.Id, r.Name))
			.ToListAsync();

		return Page();
	}

	public async Task<IActionResult> OnPost([FromForm] string username)
	{
		if (userService.User?.FindFirstValue(ClaimTypes.ImpersonatingUserId) is {  } existing)
		{
			logger.LogWarning("User {UserId} is already impersonating {Existing}", userService.UserId, existing);
			return BadRequest("You're already impersonating someone");
		}

		if (userService.UserId is not {} currentId)
		{
			return Unauthorized();
		}

		var target = await userManager.FindByNameAsync(username);

		if (target is null)
		{
			return NotFound();
		}

		if (await userManager.IsInRoleAsync(target, RoleNames.Admin))
		{
			return Forbid();
		}

		logger.LogWarning("User {UserId} is impersonating {Target}", currentId, target.UserName);

		var addedClaims = new List<Claim>
		{
			new(ClaimTypes.ImpersonatingUserId, currentId.ToString()),
		};

		await signInManager.SignOutAsync();
		await signInManager.SignInWithClaimsAsync(target, isPersistent: false, addedClaims);

		return Routes.Pages.Index.Get().Redirect(this);
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
[UsedImplicitly]
public sealed partial class InfractionNamesJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(List<Index.RoleDto>))]
[UsedImplicitly]
public sealed partial class RoleDtoJsonContext : JsonSerializerContext;