using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Quotes.Queries;

public static class GetAll
{
	public sealed record Query : IRequest<ActionResult<List<Quote>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<Quote>>>
	{
		private readonly ApplicationDbContext _context;

		public Handler(ApplicationDbContext context)
		{
			_context = context;
		}

		public async ValueTask<ActionResult<List<Quote>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var quotes = await _context.Quotes
				.OrderBy(q => q.Id)
				.ToListAsync(cancellationToken);

			return Ok(quotes);
		}
	}
}