using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Votes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Votes.Commands;

public static class CreateVote
{
	public sealed record Command(long StoryId) : IRequest<ActionResult<Result>>;

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

			var didUserVote = await _context.Votes
				.Where(v => v.StoryId == request.StoryId)
				.Where(v => v.UserId == _uid)
				.AnyAsync(cancellationToken);

			if (didUserVote) return Ok(new Result(true));

			_context.Votes.Add(new Vote
			{
				UserId = (long)_uid,
				StoryId = request.StoryId
			});
			await _context.SaveChangesAsync(cancellationToken);

			var count = await _context.Votes
				.Where(v => v.StoryId == request.StoryId)
				.CountAsync(cancellationToken);

			return Ok(new Result(true, count));
		}
	}

	public sealed record Result(bool DidVote, int? Count = null);
}