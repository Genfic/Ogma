using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services;
using Ogma3.Services.ModeratorActionService;
using Ogma3.Services.TagCache;

namespace Ogma3.Api.V1.TagNamespaces;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapDelete("tagnamespaces")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class DeleteTagNamespace
(
	AppDbContext context,
	IModeratorActionService moderatorActionService,
	TagNamespaceAliasService aliasService,
	TagCache tagCache
)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var tns = await context.TagNamespaces
			.Where(ns => ns.Id == request.Id)
			.Select(ns => new { ns.Id, ns.Name, ns.Slug })
			.FirstOrDefaultAsync(cancellationToken);

		if (tns is null)
		{
			return TypedResults.NotFound();
		}

		var tags = await context.Tags
			.Where(t => t.NamespaceId == request.Id)
			.Select(t => new TagEntry(t.Id, t.Name, null))
			.ToArrayAsync(cancellationToken);

		var rows = await context.TagNamespaces
			.Where(ns => ns.Id == request.Id)
			.ExecuteDeleteAsync(cancellationToken);

		if (rows == 0)
		{
			return TypedResults.NotFound();
		}

		aliasService.InvalidateCache();

		foreach (var tag in tags)
		{
			await tagCache.UpdateAsync(tag with { NamespaceSlug = tns.Slug }, tag);
		}

		moderatorActionService.LogTagNamespaceDeleted(tns.Id, tns.Name, tns.Slug);

		return TypedResults.Ok(request.Id);
	}

	[Validate]
	public sealed partial record Command(long Id) : IValidationTarget<Command>;
}