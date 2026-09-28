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
public sealed partial class CreateVote(AppDbContext context, IUserService userService)
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

		// The vote row and the counter are written together so a failure between them cannot
		// leave the denormalized `VoteCount` permanently out of step with the `Votes` table.
		await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

		context.Votes.Add(new Vote
		{
			UserId = uid,
			StoryId = request.StoryId,
		});
		await context.SaveChangesAsync(cancellationToken);

		var updated = await context.Stories
			.Where(s => s.Id == request.StoryId)
			.ExecuteUpdateAsync(setters => setters
					.SetProperty(s => s.VoteCount, s => s.VoteCount + 1),
				cancellationToken);

		if (updated <= 0)
		{
			await transaction.RollbackAsync(cancellationToken);
			return TypedResults.Ok(new VoteResult(false, 0));
		}

		var count = await context.Stories
			.Where(s => s.Id == request.StoryId)
			.Select(s => s.VoteCount)
			.FirstOrDefaultAsync(cancellationToken);

		await transaction.CommitAsync(cancellationToken);

		return TypedResults.Ok(new VoteResult(true, count));
	}

	[Validate]
	public sealed partial record Command(long StoryId) : IValidationTarget<Command>;
}