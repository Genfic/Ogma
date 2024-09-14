using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Clubs.Queries;

public static class GetClubsWithStory
{
	public sealed record Query(long StoryId) : IRequest<ActionResult<List<Result>>>;

	public sealed class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<List<Result>>>
	{

		public async ValueTask<ActionResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var clubs = await context.Clubs
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