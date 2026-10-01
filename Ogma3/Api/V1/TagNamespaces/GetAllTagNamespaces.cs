using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.TagNamespaces;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.TagNamespaces;

using ReturnType = Ok<TagNamespaceDto[]>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("tagnamespaces")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class GetAllTagNamespaces(AppDbContext context)
{
	private async ValueTask<ReturnType> HandleAsync(
		Query _,
		CancellationToken cancellationToken
	)
	{
		var namespaces = await context.TagNamespaces
			.OrderBy(ns => ns.Name)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(namespaces);
	}

	public sealed record Query;
}