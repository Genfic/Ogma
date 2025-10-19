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

using ReturnType = Results<Ok<GetUserData.UserDetailsDto>, NotFound>;

[Handler]
[MapGet("admin/api/users/{name}")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class GetUserData
{
	[UsedImplicitly]
	public sealed record Query(string Name);

	private static async ValueTask<ReturnType> Handle(Query request, ApplicationDbContext context, CancellationToken cancellationToken)
	{
		var query = context.Users.AsQueryable();

		if (request.Name.StartsWith("id:", StringComparison.InvariantCultureIgnoreCase) && int.TryParse(request.Name[3..], out var id))
		{
			query = query.Where(u => u.Id == id);
		}
		else
		{
			query = query.Where(u => u.NormalizedUserName == request.Name);
		}

		var user = await query.Select(u => new UserDetailsDto
			{
				Id = u.Id,
				Name = u.UserName,
				Email = u.Email,
				Title = u.Title,
				Avatar = u.Avatar.Url,
				RoleNames = u.Roles.Select(r => r.Name),
				RegistrationDate = u.RegistrationDate,
				LastActive = u.LastActive,
				StoriesCount = u.Stories.Count,
				BlogpostsCount = u.Blogposts.Count,
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

		return user is not null ? TypedResults.Ok(user) : TypedResults.NotFound();
	}

	public sealed record UserDetailsDto
	{
		public required long Id { get; init; }
		public required string Name { get; init; } = null!;
		public required string Email { get; init; } = null!;
		public required string? Title { get; init; }
		public required string? Avatar { get; init; }
		public required DateTimeOffset RegistrationDate { get; init; }
		public required DateTimeOffset LastActive { get; init; }
		public required int StoriesCount { get; init; }
		public required int BlogpostsCount { get; init; }
		public required IEnumerable<string> RoleNames { get; init; } = null!;
		public required ICollection<InfractionDto> Infractions { get; init; } = null!;
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