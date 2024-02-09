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

namespace Ogma3.Api.V1.ShelfStories.Commands;

public static class RemoveBookFromShelf
{
	public sealed record Command(long ShelfId, long StoryId) : IRequest<ActionResult<Result>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<Result>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<Result>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var (shelfId, storyId) = request;

			var res = await _context.ShelfStories
				.Where(ss => ss.ShelfId == shelfId)
				.Where(ss => ss.StoryId == storyId)
				.ExecuteDeleteAsync(cancellationToken);

			await _context.SaveChangesAsync(cancellationToken);
			return res > 0 ? Ok(new Result(shelfId, storyId)) : NotFound();
		}
	}

	public sealed record Result(long ShelfId, long StoryId);
}