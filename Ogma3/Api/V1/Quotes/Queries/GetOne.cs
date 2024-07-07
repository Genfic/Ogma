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

public static class GetOne
{
	public sealed record Query(long Id) : IRequest<ActionResult<QuoteDto>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<QuoteDto>>
	{
		public async ValueTask<ActionResult<QuoteDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var quote = await context.Quotes
				.Where(q => q.Id == request.Id)
				.ProjectToDto()
				.FirstOrDefaultAsync(cancellationToken);

			return quote is null
				? NotFound()
				: Ok(quote);
		}
	}
}