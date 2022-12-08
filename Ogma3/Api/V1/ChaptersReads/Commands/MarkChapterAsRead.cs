using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ChaptersReads.Commands;

public static class MarkChapterAsRead
{
	public sealed record Command(long Chapter, long Story) : IRequest<ActionResult<HashSet<long>>>;

	public class MarkChapterAsReadHandler : BaseHandler, IRequestHandler<Command, ActionResult<HashSet<long>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public MarkChapterAsReadHandler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService?.User?.GetNumericId();
		}

		public async Task<ActionResult<HashSet<long>>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var (chapter, story) = request;

			var chaptersReadObj = await _context.ChaptersRead
				.Where(cr => cr.StoryId == story)
				.Where(cr => cr.UserId == _uid)
				.FirstOrDefaultAsync(cancellationToken);

			if (chaptersReadObj is null)
			{
				var result = _context.ChaptersRead.Add(new ChaptersRead
				{
					StoryId = story,
					UserId = (long)_uid,
					Chapters = new HashSet<long> { chapter }
				});
				chaptersReadObj = result.Entity;
			}
			else
			{
				chaptersReadObj.Chapters.Add(chapter);
			}

			await _context.SaveChangesAsync(cancellationToken);
			return Ok(chaptersReadObj.Chapters);
		}
	}
}