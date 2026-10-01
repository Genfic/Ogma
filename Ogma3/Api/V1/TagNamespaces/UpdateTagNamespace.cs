using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services;
using Ogma3.Services.ModeratorActionService;
using Ogma3.Services.TagCache;

namespace Ogma3.Api.V1.TagNamespaces;

using ReturnType = Results<Conflict<string>, Ok, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapPut("tagnamespaces")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateTagNamespace
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
		var current = await context.TagNamespaces
			.Where(ns => ns.Id == request.Id)
			.Select(ns => new { ns.Id, ns.Name, ns.Slug, ns.Alias })
			.FirstOrDefaultAsync(cancellationToken);

		if (current is null)
		{
			return TypedResults.NotFound();
		}

		var name = request.Name.Trim();
		var slug = TagNamespaceHelpers.NormalizeSlug(request.Slug ?? name);
		var alias = TagNamespaceHelpers.NormalizeAlias(request.Alias);
		var color = TagNamespaceHelpers.NormalizeColor(request.Color);
		var description = TagNamespaceHelpers.TrimToNull(request.Description);

		TagNamespaceHelpers.EnsureNormalizedFields(name, slug, color);

		try
		{
			var rows = await ((Func<CancellationToken, Task<int>>)(ct => context.TagNamespaces.Where(ns => ns.Id == request.Id)
				.ExecuteUpdateAsync(setters => setters.SetProperty(ns => ns.Name, name)
					.SetProperty(ns => ns.Slug, slug)
					.SetProperty(ns => ns.Alias, alias)
					.SetProperty(ns => ns.Color, color)
					.SetProperty(ns => ns.Description, description), ct)))(cancellationToken);

			if (rows == 0)
			{
				return TypedResults.NotFound();
			}
		}
		catch (Exception e) when (e.IsPostgresException(PostgresErrorCodes.UniqueViolation))
		{
			return TypedResults.Conflict("A tag namespace with that name, slug or alias already exists");
		}

		aliasService.InvalidateCache();

		if (!string.Equals(current.Slug, slug, StringComparison.Ordinal))
		{
			var tags = await context.Tags
				.Where(t => t.NamespaceId == request.Id)
				.Select(t => new TagEntry(t.Id, t.Name, slug))
				.ToArrayAsync(cancellationToken);

			foreach (var tag in tags)
			{
				await tagCache.UpdateAsync(tag with { NamespaceSlug = current.Slug }, tag);
			}
		}

		moderatorActionService.LogTagNamespaceUpdated(request.Id, name, slug, alias);

		return TypedResults.Ok();
	}

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		public required long Id { get; init; }

		[NotEmpty]
		[MinLength(CTConfig.TagNamespace.MinNameLength)]
		[MaxLength(CTConfig.TagNamespace.MaxNameLength)]
		public required string Name { get; init; }

		[MinLength(CTConfig.TagNamespace.MinSlugLength)]
		[MaxLength(CTConfig.TagNamespace.MaxSlugLength)]
		public string? Slug { get; init; }

		[MaxLength(CTConfig.TagNamespace.MaxAliasLength)]
		public string? Alias { get; init; }

		[MaxLength(CTConfig.TagNamespace.ColorLength)]
		public string? Color { get; init; }

		[MaxLength(CTConfig.TagNamespace.MaxDescLength)]
		public string? Description { get; init; }
	}
}