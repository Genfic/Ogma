using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;
using Ogma3.Data;
using Ogma3.Data.TagNamespaces;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services;
using Ogma3.Services.ModeratorActionService;

namespace Ogma3.Api.V1.TagNamespaces;

using ReturnType = Results<Conflict<string>, CreatedAtRoute<TagNamespaceDto>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("tagnamespaces")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class CreateTagNamespace
(
	AppDbContext context,
	IModeratorActionService moderatorActionService,
	TagNamespaceAliasService aliasService
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
		var name = request.Name.Trim();
		var slug = TagNamespaceHelpers.NormalizeSlug(request.Slug ?? name);
		var alias = TagNamespaceHelpers.NormalizeAlias(request.Alias);
		var color = TagNamespaceHelpers.NormalizeColor(request.Color);
		var description = TagNamespaceHelpers.TrimToNull(request.Description);

		TagNamespaceHelpers.EnsureNormalizedFields(name, slug, color);

		var tns = new TagNamespace
		{
			Name = name,
			Slug = slug,
			Alias = alias,
			Color = color,
			Description = description,
		};

		context.TagNamespaces.Add(tns);

		try
		{
			await context.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e) when (e.IsPostgresException(PostgresErrorCodes.UniqueViolation))
		{
			return TypedResults.Conflict("A tag namespace with that name, slug or alias already exists");
		}

		aliasService.InvalidateCache();
		moderatorActionService.LogTagNamespaceCreated(tns.Id, tns.Name, tns.Slug, tns.Alias);

		return TypedResults.CreatedAtRoute(tns.ToDto(), nameof(GetSingleTagNamespace), new GetSingleTagNamespace.Query(tns.Id));
	}

	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
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