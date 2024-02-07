using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Infractions.Queries;

public static class GetUserInfractions
{
	public sealed record Query(long UserId) : IRequest<ActionResult<List<Result>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<Result>>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async ValueTask<ActionResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var infractions = await _context.Infractions
				.Where(i => i.UserId == request.UserId)
				.Select(i => new Result(i.Id, i.ActiveUntil, i.RemovedAt != null, i.Reason))
				.ToListAsync(cancellationToken);

			return Ok(infractions);
		}
	}

	public sealed record Result(long Id, DateTime ActiveUntil, bool Removed, string Reason);
}