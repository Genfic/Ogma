using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.SignIn;

[Handler]
[MapGet("api/signin")]
public static partial class GetSignInData
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint) => endpoint
		.ProducesValidationProblem();

	[Validate]
	public sealed partial record Query(string Name) : IValidationTarget<Query>;

	private static async ValueTask<Ok<Result>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var user = await context.Users
			.Where(u => u.NormalizedUserName == request.Name)
			.Select(u => new Result(u.Avatar.Url, u.Title))
			.FirstOrDefaultAsync(cancellationToken);

		return TypedResults.Ok(user);
	}

	public sealed record Result(string Avatar, string? Title);
}