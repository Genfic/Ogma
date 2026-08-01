using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Areas.Admin.Pages.Users;

public sealed class DetailsModel(
	ApplicationDbContext context,
	OgmaUserManager userManager,
	SignInManager<OgmaUser> signInManager,
	ILogger<DetailsModel> logger) : PageModel
{
	public UserDetailsDto? Details { get; set; }
	public List<RoleDto> Roles { get; set; } = [];

	public async Task<IActionResult> OnGetAsync(string? name)
	{
		var query = context.Users.AsQueryable();

		if (name is null)
		{
			return Page();
		}

		Roles = await context.Roles
			.Select(r => new RoleDto(r.Id, r.Name))
			.ToListAsync();

		if (name.StartsWith("id:", StringComparison.InvariantCultureIgnoreCase) && int.TryParse(name[3..], out var id))
		{
			query = query.Where(u => u.Id == id);
		}
		else
		{
			query = query.Where(u => u.NormalizedUserName == name);
		}

		Details = await query.Select(u => new UserDetailsDto
			{
				Id = u.Id,
				Name = u.UserName,
				Email = u.Email,
				Title = u.Title,
				Avatar = u.Avatar.Url,
				RoleNames = u.Roles.Select(r => r.Name),
				RegistrationDate = u.RegistrationDate,
				LastActive = u.LastActive,
				StoriesCount = u.Stories.Count,
				BlogpostsCount = u.Blogposts.Count,
			})
			.FirstOrDefaultAsync();

		return Page();
	}

	[BindProperty]
	public List<long> GivenRoles { get; set; } = [];

	public async Task<IActionResult> OnPostAsync([FromForm] long userId)
	{
		if (User.GetNumericId() is not {} uid || User.GetUsername() is not {} username)
		{
			ModelState.AddModelError("", "User not logged in.");
			return Page();
		}

		var user = await context.Users
			.Where(u => u.Id == userId)
			.Select(u => new
			{
				u.Id,
				u.UserName,
				Roles = u.Roles.Select(r => r.Id).ToArray(),
			})
			.FirstOrDefaultAsync();

		if (user is null)
		{
			ModelState.AddModelError("", "User not found.");
			return Page();
		}

		var route = Routes.Areas.Admin.Pages.Users_Details.Get(user.UserName);

		logger.LogInformation("Replacing user {Name} roles from {Old} with {New} (identical: {Identical})",
			user.UserName, user.Roles.ToHashSet(), GivenRoles, user.Roles.ToHashSet().SetEquals(GivenRoles));

		if (user.Roles.ToHashSet().SetEquals(GivenRoles))
		{
			return route.Redirect(this);
		}

		await context.UserRoles
			.Where(r => r.UserId == user.Id)
			.ExecuteDeleteAsync();

		await using var transaction = await context.Database.BeginTransactionAsync();

		await context.UserRoles
			.Where(ur => ur.UserId == user.Id)
			.ExecuteDeleteAsync();

		context.UserRoles
			.AddRange(GivenRoles.Select(r => new UserRole
			{
				UserId = user.Id,
				RoleId = r,
			}));

		context.ModeratorActions.Add(new ModeratorAction
		{
			StaffMemberId = uid,
			Description = ModeratorActionTemplates.UserRolesChanged(user.UserName, user.Id, username, user.Roles, [..GivenRoles]),
		});

		await context.SaveChangesAsync();

		await transaction.CommitAsync();

		return route.Redirect(this);
	}

	public async Task<IActionResult> OnPostImpersonateAsync([FromForm] string username)
	{
		if (string.IsNullOrEmpty(username))
		{
			return RedirectToPage();
		}

		if (User.FindFirstValue(ClaimTypes.ImpersonatingUserId) is {  } existing)
		{
			logger.LogWarning("User {UserId} is already impersonating {Existing}", User.GetNumericId(), existing);
			return BadRequest("You're already impersonating someone");
		}

		if (User.GetNumericId() is not {} currentId)
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
		public required string Name { get; init; }
		public required string Email { get; init; }
		public required string? Title { get; init; }
		public required string? Avatar { get; init; }
		public required DateTimeOffset RegistrationDate { get; init; }
		public required DateTimeOffset LastActive { get; init; }
		public required int StoriesCount { get; init; }
		public required int BlogpostsCount { get; init; }
		public required IEnumerable<string> RoleNames { get; init; }
	}

	public sealed record RoleDto(long Id, string Name);
}