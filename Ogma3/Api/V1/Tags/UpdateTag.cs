using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.ServiceRegistrations;
using Utils.Extensions;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<Ok, NotFound, Conflict<string>>;

[Handler]
[MapPut("api/tags")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateTag
{
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


	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		// Different ID but the same (name, namespace) tuple means update would make the tag a duplicate
		var duplicateExists = await context.Tags
			.Where(t => t.Id != request.Id)
			.Where(t => t.Name == request.Name && t.Namespace == request.Namespace)
			.AnyAsync(cancellationToken);

		if (duplicateExists) return TypedResults.Conflict($"Tag {request.Name} already exists in the {request.Namespace} namespace.");

		var res = await context.Tags
			.Where(t => t.Id == request.Id)
			.ExecuteUpdateAsync(tag => tag
					.SetProperty(t => t.Name, request.Name)
					.SetProperty(t => t.Slug, request.Name.Friendlify('_'))
					.SetProperty(t => t.Description, request.Description)
					.SetProperty(t => t.Namespace, request.Namespace),
				cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}