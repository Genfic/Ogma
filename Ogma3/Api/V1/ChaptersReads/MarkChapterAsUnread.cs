using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ChaptersReads;

using ReturnType = Results<UnauthorizedHttpResult, Ok<HashSet<long>>, NoContent>;

[Handler]
[MapDelete("api/chaptersread")]
public static partial class MarkChapterAsUnread
{
	[UsedImplicitly]
	public sealed record Command(long Chapter, long Story);

	private static async ValueTask<ReturnType> HandleAsync(
		[FromBody] Command request,
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

		if (chaptersReadObj is null) return TypedResults.NoContent();

		chaptersReadObj.Chapters.Remove(chapter);

		if (chaptersReadObj.Chapters.Count < 1)
		{
			context.ChaptersRead.Remove(chaptersReadObj);
		}

		await context.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok(chaptersReadObj.Chapters.ToHashSet());
	}
}