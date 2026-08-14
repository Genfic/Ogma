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

using ReturnType = Results<Conflict<string>, CreatedAtRoute<TagDto>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("tags")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed partial class CreateTag(AppDbContext context, TagCache cache)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		var tagExist = await context.Tags
			.Where(t => t.Name == request.Name && t.NamespaceId == request.NamespaceId)
			.AnyAsync(cancellationToken);

		if (tagExist)
		{
			return TypedResults.Conflict($"Tag {request.Name} already exists in namespace ID={request.NamespaceId}");
		}

		var tag = new Tag
		{
			Name = request.Name,
			Slug = request.Name.Friendlify('_'),
			Description = request.Description,
			NamespaceId = request.NamespaceId,
		};
		context.Tags.Add(tag);
		await context.SaveChangesAsync(cancellationToken);

		var nsName = await context.TagNamespaces
			.Where(n => n.Id == tag.NamespaceId)
			.Select(n => n.Name)
			.FirstOrDefaultAsync(cancellationToken);

		await cache.AddAsync(new(tag.Id, tag.Slug, nsName));

		return TypedResults.CreatedAtRoute(tag.ToDto(), nameof(GetSingleTag), new GetSingleTag.Query(tag.Id));
	}

	[Validate]
	public sealed partial record Command
	(
		[property: MinLength(CTConfig.Tag.MinNameLength)]
		[property: MaxLength(CTConfig.Tag.MaxNameLength)]
		string Name,
		[property: MaxLength(CTConfig.Tag.MaxDescLength)]
		string? Description,
		long? NamespaceId
	) : IValidationTarget<Command>;
}