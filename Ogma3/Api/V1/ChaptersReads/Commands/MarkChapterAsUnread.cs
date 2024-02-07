using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ChaptersReads.Commands;

public static class MarkChapterAsUnread
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

			if (chaptersReadObj is null) return Ok();

			chaptersReadObj.Chapters.Remove(chapter);

			if (chaptersReadObj.Chapters.Count < 1)
			{
				context.ChaptersRead.Remove(chaptersReadObj);
			}

			await context.SaveChangesAsync(cancellationToken);
			return Ok(chaptersReadObj.Chapters);
		}
	}
}