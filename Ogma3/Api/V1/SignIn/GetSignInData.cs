using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Services.GeneratedImagesService;

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
		OgmaConfig config,
		GeneratedImagesService genImg,
		CancellationToken cancellationToken
	)
	{
		var user = await context.Users
			.Where(u => u.NormalizedUserName == request.Name)
			.Select(u => new {u.Avatar.Url, u.Title})
			.FirstOrDefaultAsync(cancellationToken);

		if (user is null)
		{
			return TypedResults.Ok(new Result(genImg.GenerateAvatarUrl(request.Name)));
		}

		var res = new Result(Path.Join(config.Cdn, user.Url), user.Title);

		return TypedResults.Ok(res);
	}

	public sealed record Result(string Avatar, string? Title = null);
}