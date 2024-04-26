using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Infractions.Commands;

public static class DeactivateInfraction
{
	public sealed record Command(long InfractionId) : IRequest<ActionResult<Response>>;

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command, ActionResult<Response>>
	{
		public async ValueTask<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not { } _uid) return Unauthorized();

			var infraction = await context.Infractions
				.Where(i => i.Id == request.InfractionId)
				.FirstOrDefaultAsync(cancellationToken);

			if (infraction is null) return NotFound();

			infraction.RemovedAt = DateTime.Now;
			infraction.RemovedById = _uid;

			await context.SaveChangesAsync(cancellationToken);

			return Ok(new Response(infraction.Id, _uid, infraction.UserId));
		}
	}

	public sealed record Response(long Id, long IssuedBy, long IssuedAgainst);
}