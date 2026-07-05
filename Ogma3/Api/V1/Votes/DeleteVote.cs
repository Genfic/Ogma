using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<VoteResult>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("votes")]
[Authorize]
public sealed partial class DeleteVote(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		[FromBody] Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var res = await context.Votes
			.Where(v => v.StoryId == request.StoryId)
			.Where(v => v.UserId == uid)
			.ExecuteDeleteAsync(cancellationToken);

		if (res <= 0) return TypedResults.NotFound();

		var count = await context.Votes
			.Where(v => v.StoryId == request.StoryId)
			.CountAsync(cancellationToken);

		await context.Stories.ExecuteUpdateAsync(setters => setters
				.SetProperty(s => s.VoteCount, count),
			cancellationToken);

		return TypedResults.Ok(new VoteResult(false, count));
	}

	[Validate]
	public sealed partial record Command(long StoryId) : IValidationTarget<Command>;
}