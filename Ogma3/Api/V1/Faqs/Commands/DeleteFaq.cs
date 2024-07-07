using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Faqs.Commands;

public static class DeleteFaq
{
	public sealed record Command(long Id) : IRequest<ActionResult<long>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Command, ActionResult<long>>
	{
		public async ValueTask<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
		{
			var res = await context.Faqs
				.Where(f => f.Id == request.Id)
				.ExecuteDeleteAsync(cancellationToken);

			return res > 0 ? Ok(request.Id) : NotFound();
		}
	}
}