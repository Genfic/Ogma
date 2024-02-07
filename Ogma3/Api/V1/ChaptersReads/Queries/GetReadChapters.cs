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

namespace Ogma3.Api.V1.ChaptersReads.Queries;

public static class GetReadChapters
{
	public sealed record Query(long Id) : IRequest<ActionResult<HashSet<long>>>;

	public class Handler(ApplicationDbContext context, IUserService userService)
		: BaseHandler, IRequestHandler<Query, ActionResult<HashSet<long>>>
	{
		public async ValueTask<ActionResult<HashSet<long>>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();
			
			var chaptersRead = await context.ChaptersRead
				.Where(cr => cr.StoryId == request.Id)
				.Where(cr => cr.UserId == uid)
				.Select(cr => cr.Chapters)
				.FirstOrDefaultAsync(cancellationToken);

			return Ok(chaptersRead ?? []);
		}
	}
}