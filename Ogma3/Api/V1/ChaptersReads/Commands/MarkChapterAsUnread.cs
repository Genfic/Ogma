using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;
using Serilog;

namespace Ogma3.Api.V1.ChaptersReads.Commands;

public static class MarkChapterAsUnread
{
	public sealed record Command(long Chapter, long Story) : IRequest<ActionResult<Response>>;

	public class MarkChapterAsReadHandler : BaseHandler, IRequestHandler<Command, ActionResult<Response>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public MarkChapterAsReadHandler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService?.User?.GetNumericId();
		}

		public async Task<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var (chapter, story) = request;

			var chaptersReadObj = await _context.ChaptersRead
				.Where(cr => cr.StoryId == story)
				.Where(cr => cr.UserId == _uid)
				.FirstOrDefaultAsync(cancellationToken);

			if (chaptersReadObj is null) return Ok();

			chaptersReadObj.Chapters.Remove(chapter);
			_context.Entry(chaptersReadObj).State = EntityState.Modified;

			if (chaptersReadObj.Chapters.Count < 1)
			{
				_context.ChaptersRead.Remove(chaptersReadObj);
			}

			// Save
			try
			{
				await _context.SaveChangesAsync(cancellationToken);
				return Ok(new Response(chaptersReadObj.Chapters));
			}
			catch (Exception e)
			{
				Log.Error(e, "Exception occurred when marking chapter {Chapter} as unread by {User}", chapter,
					_uid);
				return ServerError("Database remove error");
			}
		}
	}

	public sealed record Response(HashSet<long> Read);
}