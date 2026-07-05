using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<VoteResult>>;

[Handler]
[MapGet("api/votes/{storyId:long}")]
public sealed partial class GetVotes(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		var count = await context.Stories
			.Where(s => s.Id == request.StoryId)
			.Select(s => s.VoteCount)
			.FirstOrDefaultAsync(cancellationToken);

		var didUserVote = userService.UserId switch
		{
			{} uid => await context.Votes
				.Where(v => v.StoryId == request.StoryId)
				.Where(v => v.UserId == uid)
				.AnyAsync(cancellationToken),
			_ => false,
		};

		return TypedResults.Ok(new VoteResult(didUserVote, count));
	}

	[Validate]
	public sealed partial record Query(long StoryId) : IValidationTarget<Query>;
}