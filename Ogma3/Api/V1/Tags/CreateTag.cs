using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Tags;

using ReturnType = Results<Conflict<string>, CreatedAtRoute<TagDto>>;

[Handler]
[MapPost("api/tags")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateTag
{
	public sealed record Command(string Name, string? Description, ETagNamespace? Namespace);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(t => t.Name)
				.MinimumLength(CTConfig.Tag.MinNameLength)
				.MaximumLength(CTConfig.Tag.MaxNameLength);
			RuleFor(t => t.Description)
				.MaximumLength(CTConfig.Tag.MaxDescLength);
		}
	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var tagExist = await context.Tags
			.Where(t => t.Name == request.Name && t.Namespace == request.Namespace)
			.AnyAsync(cancellationToken);

		if (tagExist) return TypedResults.Conflict($"Tag {request.Name} already exists in the {request.Namespace} namespace.");

		var tag = new Tag
		{
			Name = request.Name,
			Description = request.Description,
			Namespace = request.Namespace,
		};
		context.Tags.Add(tag);
		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.CreatedAtRoute(tag.ToDto(), nameof(GetSingleTag), new GetSingleTag.Query(tag.Id));
	}
}