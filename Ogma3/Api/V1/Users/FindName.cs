using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Users;

using ReturnType = Results<Ok<string[]>, UnprocessableEntity<string>>;

[Handler]
[MapGet("api/users/names")]
[Authorize(AuthorizationPolicies.RequireStaffRole)]
public static partial class FindName
{
	[Validate]
	public sealed partial record Query(string Name) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		if (request.Name.Length < 3) return TypedResults.UnprocessableEntity("You need at least 3 characters");

		var name = request.Name.Normalize().ToUpperInvariant();

		var names = await context.Users
			.Where(u => EF.Functions.Like(u.NormalizedUserName, $"%{name}%"))
			.Select(u => u.UserName)
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(names);
	}
}