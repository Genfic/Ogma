using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Faqs.Queries;

public static class GetSingleFaq
{
	public sealed record Query(long FaqId) : IRequest<ActionResult<FaqDto>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<FaqDto>>
	{
		public async ValueTask<ActionResult<FaqDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var faq = await context.Faqs
				.Where(f => f.Id == request.FaqId)
				.ProjectToDto()
				.FirstOrDefaultAsync(cancellationToken);

			return faq is null ? NotFound() : Ok(faq);
		}
	}
}