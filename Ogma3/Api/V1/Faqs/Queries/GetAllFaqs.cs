using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Faqs.Queries;

public static class GetAllFaqs
{
	public sealed record Query : IRequest<ActionResult<List<Faq>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<Faq>>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async ValueTask<ActionResult<List<Faq>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var faqs = await _context.Faqs
				.AsNoTracking()
				.ToListAsync(cancellationToken);

			return Ok(faqs);
		}
	}
}