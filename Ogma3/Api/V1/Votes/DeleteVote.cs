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
public sealed partial class DeleteVote(AppDbContext context, IUserService userService)
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

		// The transaction must be opened *before* the delete: ExecuteDeleteAsync outside a
		// transaction auto-commits, which would leave the vote gone even when the story
		// adjustment below fails and we return NotFound.
		await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

		var res = await context.Votes
			.Where(v => v.StoryId == request.StoryId)
			.Where(v => v.UserId == uid)
			.ExecuteDeleteAsync(cancellationToken);

		if (res <= 0)
		{
			await transaction.RollbackAsync(cancellationToken);
			return TypedResults.NotFound();
		}

		var updated = await context.Stories
			.Where(s => s.Id == request.StoryId)
			.ExecuteUpdateAsync(setters => setters
					.SetProperty(s => s.VoteCount, s => s.VoteCount > 0 ? s.VoteCount - 1 : 0),
				cancellationToken);

		if (updated <= 0)
		{
			await transaction.RollbackAsync(cancellationToken);
			return TypedResults.NotFound();
		}

		var count = await context.Stories
			.Where(s => s.Id == request.StoryId)
			.Select(s => s.VoteCount)
			.FirstOrDefaultAsync(cancellationToken);

		await transaction.CommitAsync(cancellationToken);

		return TypedResults.Ok(new VoteResult(false, count));
	}

	[Validate]
	public sealed partial record Command(long StoryId) : IValidationTarget<Command>;
}