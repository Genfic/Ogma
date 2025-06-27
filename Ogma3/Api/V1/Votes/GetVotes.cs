using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<VoteResult>>;

[Handler]
[MapGet("api/votes/{storyId:long}")]
public static partial class GetVotes
{
	[Validate]
	public sealed partial record Query(long StoryId) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		var count = await context.Votes
			.Where(v => v.StoryId == request.StoryId)
			.CountAsync(cancellationToken);

		var didUserVote = userService.User?.GetNumericId() switch
		{
			{} uid => await context.Votes
				.Where(v => v.StoryId == request.StoryId)
				.Where(v => v.UserId == uid)
				.AnyAsync(cancellationToken),
			_ => false,
		};

		return TypedResults.Ok(new VoteResult(didUserVote, count));
	}
}