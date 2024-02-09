using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories.Queries;

public static class CheckUserHasStoryOnShelves
{
	public sealed record Query(long ShelfId) : IRequest<ActionResult<List<long>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<long>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<List<long>>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var shelves = await _context.Shelves
				.Where(s => s.OwnerId == _uid)
				.Where(s => s.Stories.Any(x => x.Id == request.ShelfId))
				.Select(s => s.Id)
				.ToListAsync(cancellationToken);

			return Ok(shelves);
		}
	}
}