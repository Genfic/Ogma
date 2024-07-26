using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Clubs.Queries;

public static class GetClubsWithStory
{
	public sealed record Query(long StoryId) : IRequest<ActionResult<List<Result>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<Result>>>
	{
		private readonly ApplicationDbContext _context;

		public Handler(ApplicationDbContext context)
		{
			_context = context;
		}

		public async ValueTask<ActionResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var clubs = await _context.Clubs
				.Where(c => c.Folders
					.Any(f => f.Stories
						.Any(s => s.Id == request.StoryId)))
				.Select(c => new Result(c.Id, c.Name, c.Icon))
				.ToListAsync(cancellationToken);

			return Ok(clubs);
		}
	}

	public sealed record Result(long Id, string Name, string Icon);
}