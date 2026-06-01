using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ChaptersReads;

using ReturnType = Results<UnauthorizedHttpResult, Ok<HashSet<long>>>;

[Handler]
[MapPost("api/chaptersread")]
[Authorize]
public sealed partial class MarkChapterAsRead(ApplicationDbContext context, IUserService userService)
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

		var (chapter, story) = request;

		var chaptersReadObj = await context.ChaptersRead
			.Where(cr => cr.StoryId == story)
			.Where(cr => cr.UserId == uid)
			.FirstOrDefaultAsync(cancellationToken);

		if (chaptersReadObj is null)
		{
			var cr = new ChaptersRead
			{
				StoryId = story,
				UserId = uid,
			};
			cr.AddChapter(chapter);
			var result = context.ChaptersRead.Add(cr);
			chaptersReadObj = result.Entity;
		}
		else
		{
			chaptersReadObj.AddChapter(chapter);
		}

		await context.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok(chaptersReadObj.Chapters.ToHashSet());
	}

	[Validate]
	public sealed partial record Command(long Chapter, long Story) : IValidationTarget<Command>;
}