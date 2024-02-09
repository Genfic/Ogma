using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.InviteCodes.Commands;

public static class DeleteInviteCode
{
	public sealed record Command(long CodeId) : IRequest<ActionResult<long>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<long>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async ValueTask<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
		{
			var res = await _context.InviteCodes
				.Where(ic => ic.Id == request.CodeId)
				.ExecuteDeleteAsync(cancellationToken);
			
			return res > 0 ? Ok(request.CodeId) : NotFound();
		}
	}
}