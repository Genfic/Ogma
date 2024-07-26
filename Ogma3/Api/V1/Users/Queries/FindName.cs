using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Users.Queries;

public static class FindName
{
	public sealed record Query(string Name) : IRequest<ActionResult<List<string>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<string>>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async ValueTask<ActionResult<List<string>>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (request.Name.Length < 3) return UnprocessableEntity("You need at least 3 characters");

			var name = request.Name.Normalize().ToUpperInvariant();

			var names = await _context.Users
				.Where(u => EF.Functions.Like(u.NormalizedUserName, $"%{name}%"))
				.Select(u => u.UserName)
				.ToListAsync(cancellationToken);

			return Ok(names);
		}
	}
}