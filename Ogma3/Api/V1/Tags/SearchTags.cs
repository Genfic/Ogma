using System.Linq.Expressions;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGet("api/tags/search")]
public static partial class SearchTags
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint) => endpoint
		.ProducesValidationProblem();

	[Validate]
	public sealed partial record Query(string SearchString) : IValidationTarget<Query>;

	private static async ValueTask<Results<Ok<TagDto[]>, BadRequest>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		// TODO: Could probably have been done better
		// We need to use `EF.Functions.Collate` because Postgres versions < 18 don't support `ILIKE` for
		// columns with non-deterministic collations.
		// ReSharper disable EntityFramework.ClientSideDbFunctionCall
 #pragma warning disable NEEG004
		Expression<Func<Tag, bool>>? search = request.SearchString.Split(':') switch
		{
			[{ Length: > 0 } ns, { Length: > 0 } name] => t
				=> EF.Functions.ILike(EF.Functions.Collate(t.Name, "C"), $"%{name}%")
				   && EF.Functions.ILike(EF.Functions.Collate(t.Namespace.ToString() ?? string.Empty, "C"), $"%{ns}%"),

			[{ Length: > 0 } ns, _]
				=> t => EF.Functions.ILike(EF.Functions.Collate(t.Namespace.ToString() ?? string.Empty, "C"), $"%{ns}%"),

			[_, { Length: > 0 } name]
				=> t => EF.Functions.ILike(EF.Functions.Collate(t.Name, "C"), $"%{name}%"),

			[{ Length: > 0 } q] => t
				=> EF.Functions.ILike(EF.Functions.Collate(t.Name, "C"), $"%{q}%")
				   || EF.Functions.ILike(EF.Functions.Collate(t.Namespace.ToString() ?? string.Empty, "C"), $"%{q}%"),

			_ => null,
		};
 #pragma warning restore NEEG004
		// ReSharper enable EntityFramework.ClientSideDbFunctionCall

		if (search is null) return TypedResults.BadRequest();

		var tags = await context.Tags
			.Where(search)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(tags);
	}
}