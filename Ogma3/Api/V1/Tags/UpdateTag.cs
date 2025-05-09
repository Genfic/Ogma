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

using ReturnType = Results<Ok, NotFound, Conflict<string>>;

[Handler]
[MapPut("api/tags")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateTag
{
	public sealed record Command(long Id, string? Name, string? Description, ETagNamespace? Namespace);

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
		// Different ID but the same (name, namespace) tuple means update would make the tag a duplicate
		var duplicateExists = await context.Tags
			.Where(t => t.Id != request.Id)
			.Where(t => t.Name == request.Name && t.Namespace == request.Namespace)
			.AnyAsync(cancellationToken);

		if (duplicateExists) return TypedResults.Conflict($"Tag {request.Name} already exists in the {request.Namespace} namespace.");

		var res = await context.Tags
			.Where(t => t.Id == request.Id)
			.ExecuteUpdateAsync(tag => tag
					.SetProperty(t => t.Name, t => request.Name ?? t.Name)
					.SetProperty(t => t.Description, request.Description)
					.SetProperty(t => t.Namespace, request.Namespace),
				cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}