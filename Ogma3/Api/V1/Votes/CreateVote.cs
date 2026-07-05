using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Votes;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<VoteResult>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("votes")]
[Authorize]
public sealed partial class CreateVote(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var didUserVote = await context.Votes
			.Where(v => v.StoryId == request.StoryId)
			.Where(v => v.UserId == uid)
			.AnyAsync(cancellationToken);

		if (didUserVote) return TypedResults.Ok(new VoteResult(true));

		context.Votes.Add(new Vote
		{
			UserId = uid,
			StoryId = request.StoryId,
		});
		await context.SaveChangesAsync(cancellationToken);

		var count = await context.Votes
			.Where(v => v.StoryId == request.StoryId)
			.CountAsync(cancellationToken);

		await context.Stories.ExecuteUpdateAsync(setters => setters
				.SetProperty(s => s.VoteCount, count),
			cancellationToken);

		return TypedResults.Ok(new VoteResult(true, count));
	}

	[Validate]
	public sealed partial record Command(long StoryId) : IValidationTarget<Command>;
}