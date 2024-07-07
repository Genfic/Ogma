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
	public sealed record Query : IRequest<ActionResult<List<QuoteDto>>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<List<QuoteDto>>>
	{
		public async ValueTask<ActionResult<List<QuoteDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var quotes = await context.Quotes
				.OrderBy(q => q.Id)
				.ProjectToDto()
				.ToListAsync(cancellationToken);

			return Ok(quotes);
		}
	}
}