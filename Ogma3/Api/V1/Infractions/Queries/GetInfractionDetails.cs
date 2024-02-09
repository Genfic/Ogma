using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Infractions.Queries;

public static class GetInfractionDetails
{
	public sealed record Query(long InfractionId) : IRequest<ActionResult<Result>>;

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Query, ActionResult<Result>>
	{
		public async ValueTask<ActionResult<Result>> Handle(Query request, CancellationToken cancellationToken)
		{
			var infraction = await context.Infractions
				.Where(i => i.Id == request.InfractionId)
				.Select(i => new Result
				{
					Id = i.Id,
					UserName = i.User.UserName,
					UserId = i.UserId,
					IssueDate = i.IssueDate,
					ActiveUntil = i.ActiveUntil,
					RemovedAt = i.RemovedAt,
					Reason = i.Reason,
					Type = i.Type,
					IssuedByName = i.IssuedBy.UserName,
					RemovedByName = i.RemovedBy == null ? null : i.RemovedBy.UserName
				})
				.FirstOrDefaultAsync(cancellationToken);

			if (infraction is null) return NotFound();

			return Ok(infraction);
		}
	}

	public sealed record Result
	{
		public required long Id { get; init; }
		public required string UserName { get; init; }
		public required long UserId { get; init; }
		public required DateTime IssueDate { get; init; }
		public required DateTime ActiveUntil { get; init; }
		public required DateTime? RemovedAt { get; init; }
		public required string Reason { get; init; }
		public required InfractionType Type { get; init; }
		public required string IssuedByName { get; init; }
		public required string? RemovedByName { get; init; }
	}
}