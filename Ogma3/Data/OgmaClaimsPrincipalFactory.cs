using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Data;

public sealed class OgmaClaimsPrincipalFactory
(
	UserManager<OgmaUser> userManager,
	RoleManager<OgmaRole> roleManager,
	IOptions<IdentityOptions> options)
	: UserClaimsPrincipalFactory<OgmaUser, OgmaRole>(userManager, roleManager, options)
{

	public override async Task<ClaimsPrincipal> CreateAsync(OgmaUser user)
	{
		var principal = await base.CreateAsync(user);

		var isStaff = await RoleManager.Roles
			.Where(r => r.IsStaff)
			.Where(r => r.Users.Any(u => u.Id == user.Id))
			.AnyAsync();

		((ClaimsIdentity?)principal.Identity)?.AddClaims([
			new(ClaimTypes.Avatar, user.Avatar),
			new(ClaimTypes.Title, user.Title ?? string.Empty),
			new(ClaimTypes.IsStaff, isStaff.ToString()),
		]);

		return principal;
	}

	public static class ClaimTypes
	{
		public const string Avatar = "Avatar";
		public const string Title = "Title";
		public const string IsStaff = "IsStaff";
	}
}