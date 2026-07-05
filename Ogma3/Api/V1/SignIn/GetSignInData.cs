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
[MapGroup<ApiGroup>]
[MapGet("signin")]
public sealed partial class GetSignInData(ApplicationDbContext context, OgmaConfig config, GeneratedImagesService genImg)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<Ok<Result>> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		var user = await context.Users
			.Where(u => u.NormalizedUserName == request.Name)
			.Select(u => new { u.Avatar.Url, u.Title })
			.FirstOrDefaultAsync(cancellationToken);

		if (user is null)
		{
			return TypedResults.Ok(new Result(genImg.GenerateAvatarUrl(request.Name)));
		}

		var res = new Result(Path.Join(config.Cdn, user.Url), user.Title);

		return TypedResults.Ok(res);
	}

	[Validate]
	public sealed partial record Query(string Name) : IValidationTarget<Query>;

	public sealed record Result(string Avatar, string? Title = null);
}