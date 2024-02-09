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

namespace Ogma3.Api.V1.Votes.Queries;

public static class GetVotes
{
	public sealed record Query(long StoryId) : IRequest<ActionResult<Result>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<Result>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<Result>> Handle(Query request, CancellationToken cancellationToken)
		{
			var count = await _context.Votes
				.Where(v => v.StoryId == request.StoryId)
				.CountAsync(cancellationToken);
			var didUserVote = await _context.Votes
				.Where(v => v.StoryId == request.StoryId)
				.Where(v => v.UserId == _uid)
				.AnyAsync(cancellationToken);

			return Ok(new Result(count, didUserVote));
		}
	}

	public sealed record Result(int Count, bool DidVote);
}