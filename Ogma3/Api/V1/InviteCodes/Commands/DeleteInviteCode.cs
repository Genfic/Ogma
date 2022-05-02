using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.InviteCodes.Commands;

public static class DeleteInviteCode
{
	public sealed record Command(long CodeId) : IRequest<ActionResult<long>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<long>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async Task<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
		{
			var code = await _context.InviteCodes
				.Where(ic => ic.Id == request.CodeId)
				.FirstOrDefaultAsync(cancellationToken);

			if (code is null) return NotFound();

			_context.InviteCodes.Remove(code);

			await _context.SaveChangesAsync(cancellationToken);
			return Ok(code.Id);
		}
	}
}