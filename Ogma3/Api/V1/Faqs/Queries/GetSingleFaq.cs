using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Faqs.Queries;

public static class GetSingleFaq
{
	public sealed record Query(long FaqId) : IRequest<ActionResult<Faq>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<Faq>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async Task<ActionResult<Faq>> Handle(Query request, CancellationToken cancellationToken)
		{
			var faq = await _context.Faqs
				.Where(f => f.Id == request.FaqId)
				.FirstOrDefaultAsync(cancellationToken);

			return Ok(faq);
		}
	}
}