using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Folders;

using ReturnType = Results<Ok<GetFolder.Result[]>, UnauthorizedHttpResult>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("folders")]
public sealed partial class GetFolder(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var folders = await context.Folders
			.Where(f => f.ClubId == request.ClubId)
			.Select(f => new Result(
				f.Id,
				f.Name,
				f.Slug,
				// We cannot use `.Any(c => c.Role <= f.AccessLevel)` here, since Postgres has no conversion between
				// enums and integers, not even a concept of an enum *being* an integer.
				f.Club.ClubMembers.First(c => c.MemberId == uid).Role <= f.AccessLevel
			))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(folders);
	}

	[Validate]
	public sealed partial record Query(long ClubId) : IValidationTarget<Query>;

	public sealed record Result(long Id, string Name, string Slug, bool CanAdd);
}