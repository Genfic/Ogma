using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ChaptersReads;

using ReturnType = Results<UnauthorizedHttpResult, Ok<HashSet<long>>>;

[Handler]
[MapGet("api/ChaptersRead/{id:long}")]
public sealed partial class GetReadChapters(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.DisableAntiforgery()
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var chaptersRead = await context.ChaptersRead
			.Where(cr => cr.StoryId == request.Id)
			.Where(cr => cr.UserId == uid)
			.Select(cr => cr.Chapters)
			.FirstOrDefaultAsync(cancellationToken);

		return TypedResults.Ok(chaptersRead?.ToHashSet() ?? []);
	}

	[Validate]
	public sealed partial record Query(long Id) : IValidationTarget<Query>;
}