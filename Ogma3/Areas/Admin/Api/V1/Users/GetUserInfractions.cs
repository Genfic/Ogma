using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Users;

using ReturnType = Results<Ok<GetUserInfractions.InfractionDetails>, NotFound>;

[Handler]
[MapGet("admin/api/user/infractions/{id}")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed partial class GetUserInfractions(ApplicationDbContext context)
{
	[UsedImplicitly]
	public sealed record Query(long Id);

	private async ValueTask<ReturnType> Handle(Query request, CancellationToken cancellationToken)
	{
		var data = await context.Users
			.Where(u => u.Id == request.Id)
			.Select(u => new InfractionDetails
			{
				UserId = u.Id,
				Infractions = u.Infractions
					.OrderByDescending(i => i.Type)
					.ThenByDescending(i => i.ActiveUntil)
					.Select(i => new InfractionDto
					{
						Id = i.Id,
						Reason = i.Reason,
						Type = i.Type,
						ActiveUntil = i.ActiveUntil,
						IssueDate = i.IssueDate,
						RemovedAt = i.RemovedAt,
						RemovedBy = i.RemovedBy != null ? i.RemovedBy.UserName : null,
					})
					.ToList(),
			})
			.FirstOrDefaultAsync(cancellationToken);

		return data is not null ? TypedResults.Ok(data) : TypedResults.NotFound();
	}

	public sealed record InfractionDetails
	{
		public required long UserId { get; init; }
		public required ICollection<InfractionDto> Infractions { get; init; }
		public InfractionType[] InfractionTypes => InfractionTypeExtensions.GetValues();
	}

	public sealed record InfractionDto
	{
		public required long Id { get; init; }
		public required DateTimeOffset IssueDate { get; init; }
		public required DateTimeOffset ActiveUntil { get; init; }
		public required DateTimeOffset? RemovedAt { get; init; }
		public required string Reason { get; init; }
		public required InfractionType Type { get; init; }
		public required string? RemovedBy { get; init; }
	}
}