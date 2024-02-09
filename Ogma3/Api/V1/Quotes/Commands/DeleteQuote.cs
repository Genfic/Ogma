using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Quotes.Commands;

public static class DeleteQuote
{
	public sealed record Command(long Id) : IRequest<ActionResult<Quote>>;

	public class CreateQuoteHandler : BaseHandler, IRequestHandler<Command, ActionResult<Quote>>
	{
		private readonly ApplicationDbContext _context;

		public CreateQuoteHandler(ApplicationDbContext context)
		{
			_context = context;
		}

		public async ValueTask<ActionResult<Quote>> Handle(Command request, CancellationToken cancellationToken)
		{
			var res = await _context.Quotes
				.Where(q => q.Id == request.Id)
				.ExecuteDeleteAsync(cancellationToken);

			return res > 0 ? Ok(request.Id) : NotFound();
		}
	}
}