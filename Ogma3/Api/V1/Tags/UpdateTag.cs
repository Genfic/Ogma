using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.TagCache;
using Utils.Extensions;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<Ok, NotFound, Conflict<string>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPut("tags")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class UpdateTag(ApplicationDbContext context, TagCache cache)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		// Different ID but the same (name, namespace) tuple means update would make the tag a duplicate
		var duplicateExists = await context.Tags
			.Where(t => t.Id != request.Id)
			.Where(t => t.Name == request.Name && t.Namespace == request.Namespace)
			.AnyAsync(cancellationToken);

		if (duplicateExists)
		{
			return TypedResults.Conflict($"Tag {request.Name} already exists in the {request.Namespace?.ToStringFast()} namespace.");
		}

		var tag = await context.Tags
			.Where(t => t.Id == request.Id)
			.Select(t => new TagEntry(t.Id, t.Name, t.Namespace))
			.FirstOrDefaultAsync(cancellationToken);

		if (tag is null)
		{
			return TypedResults.NotFound();
		}

		var res = await context.Tags
			.Where(t => t.Id == request.Id)
			.ExecuteUpdateAsync(setters => setters
					.SetProperty(t => t.Name, request.Name)
					.SetProperty(t => t.Slug, request.Name.Friendlify('_'))
					.SetProperty(t => t.Description, request.Description)
					.SetProperty(t => t.Namespace, request.Namespace)
					.SetProperty(t => t.LastChange, DateTimeOffset.UtcNow),
				cancellationToken);

		await cache.UpdateAsync(tag, new(request.Id, request.Name, request.Namespace));

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command
	(
		long Id,
		[property: MinLength(CTConfig.Tag.MinNameLength)]
		[property: MaxLength(CTConfig.Tag.MaxNameLength)]
		string Name,
		[property: MaxLength(CTConfig.Tag.MaxDescLength)]
		string? Description,
		ETagNamespace? Namespace
	) : IValidationTarget<Command>;
}