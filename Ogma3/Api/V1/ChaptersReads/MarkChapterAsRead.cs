using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ChaptersReads;

using ReturnType = Results<UnauthorizedHttpResult, Ok<HashSet<long>>>;

[Handler]
[MapPost("api/chaptersread")]
[Authorize]
public static partial class MarkChapterAsRead
{
	[UsedImplicitly]
	public sealed record Command(long Chapter, long Story);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var (chapter, story) = request;

		var chaptersReadObj = await context.ChaptersRead
			.Where(cr => cr.StoryId == story)
			.Where(cr => cr.UserId == uid)
			.FirstOrDefaultAsync(cancellationToken);

		if (chaptersReadObj is null)
		{
			var result = context.ChaptersRead.Add(new ChaptersRead
			{
				StoryId = story,
				UserId = uid,
				Chapters = [chapter],
			});
			chaptersReadObj = result.Entity;
		}
		else
		{
			// NOTE: Workaround until hashsets in #83 are fixed
			HashSet<long> chapters = [..chaptersReadObj.Chapters, chapter];
			chaptersReadObj.Chapters = chapters.ToList();
		}

		await context.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok(chaptersReadObj.Chapters.ToHashSet());
	}

}