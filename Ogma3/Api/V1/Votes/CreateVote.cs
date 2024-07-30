using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Votes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<VoteResult>>;

[Handler]
[MapPost("api/votes")]
[Authorize]
public static partial class CreateVote
{
	public sealed record Command(long StoryId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

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

		return TypedResults.Ok(new VoteResult(true, count));
	}
}