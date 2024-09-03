using System.Linq.Expressions;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGet("api/tags/search")]
public static partial class SearchTags
{
	[UsedImplicitly]
	public sealed record Query(string SearchString);

	private static async ValueTask<Results<Ok<TagDto[]>, BadRequest>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		// TODO: Could probably have been done better
		// ReSharper disable EntityFramework.ClientSideDbFunctionCall
		Expression<Func<Tag, bool>>? search = request.SearchString.Split(':') switch
		{
			[{ Length: > 0 } ns, { Length: > 0 } name] => (Tag t)
				=> EF.Functions.ILike(t.Name, $"%{name}%") && EF.Functions.ILike(t.Namespace.ToString() ?? string.Empty, $"%{ns}%"),
			
			[{ Length: > 0 } ns, { Length: <= 0 }] 
				=> (Tag t) => EF.Functions.ILike(t.Namespace.ToString() ?? string.Empty, $"%{ns}%"),
			
			[{ Length: <= 0 }, { Length: > 0 } name] 
				=> (Tag t) => EF.Functions.ILike(t.Name, $"%{name}%"),
			
			[{ Length: > 0 } q] => (Tag t) 
				=> EF.Functions.ILike(t.Name, $"%{q}%") || EF.Functions.ILike(t.Namespace.ToString() ?? string.Empty, $"%{q}%"),
			
			_ => null,
		};
		// ReSharper enable EntityFramework.ClientSideDbFunctionCall

		if (search is null) return TypedResults.BadRequest();

		var tags = await context.Tags
			.Where(search)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(tags);
	}
}