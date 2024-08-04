using Microsoft.AspNetCore.Authorization;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Infrastructure.ServiceRegistrations;

public static class AuthorizationPolicies
{
	public const string RequireAdminRole = nameof(RequireAdminRole);
	public const string RequireStaffRole = nameof(RequireStaffRole);
	
	public static AuthorizationBuilder AddAuthorizationPolicies(this IServiceCollection services)
	{
		return services.AddAuthorizationBuilder()
			.AddPolicy(RequireAdminRole, policy => policy.RequireRole(RoleNames.Admin))
			.AddPolicy(RequireStaffRole, policy => policy.RequireRole(RoleNames.Admin, RoleNames.Moderator, RoleNames.Helper));
	}
}