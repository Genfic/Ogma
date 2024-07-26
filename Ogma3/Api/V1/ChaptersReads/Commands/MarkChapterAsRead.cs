using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ChaptersReads.Commands;

public static class MarkChapterAsRead
{
	public sealed record Command(long Chapter, long Story) : IRequest<ActionResult<HashSet<long>>>;

	public class MarkChapterAsReadHandler(ApplicationDbContext context, IUserService userService)
		: BaseHandler, IRequestHandler<Command, ActionResult<HashSet<long>>>
	{
		public async ValueTask<ActionResult<HashSet<long>>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();

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
					Chapters = [chapter]
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
			return Ok(chaptersReadObj.Chapters);
		}
	}
}