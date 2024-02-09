using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Quotes.Commands;

public static class UpdateQuote
{
	public sealed record Command(long Id, string Body, string Author) : IRequest<ActionResult<bool>>;

	public class CreateQuoteHandler : BaseHandler, IRequestHandler<Command, ActionResult<bool>>
	{
		private readonly ApplicationDbContext _context;

		public CreateQuoteHandler(ApplicationDbContext context)
		{
			_context = context;
		}

		public async ValueTask<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			var (id, body, author) = request;

			var res = await _context.Quotes
				.Where(q => q.Id == id)
				.ExecuteUpdateAsync(q => q
					.SetProperty(x => x.Body, body)
					.SetProperty(x => x.Author, author), 
				cancellationToken);

			return res > 0 ? Ok() : NotFound();
		}
		
	}
}