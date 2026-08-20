using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Services;
using Utils.Extensions;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("tags/search")]
[UsedImplicitly]
public sealed partial class SearchTags(AppDbContext context, TagNamespaceAliasService aliasService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	[Validate]
	[UsedImplicitly]
	public sealed partial record Query(string SearchString) : IValidationTarget<Query>;

	private async ValueTask<Results<Ok<TagDto[]>, BadRequest>> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		var query = context.Tags.AsQueryable();

		var aliases = await aliasService.GetAliases(cancellationToken);
		var lookup = aliases.GetAlternateLookup<ReadOnlySpan<char>>();

		var searchSpan = request.SearchString
			.AsSpan()
			.Trim();
		var colon = searchSpan.IndexOf(':');

		switch (colon)
		{
			case > 0:
			{
				var name = searchSpan[(colon + 1)..].Trim().ToString();
				var nspace = searchSpan[..colon].Trim();

				if (lookup.TryGetValue(nspace, out var ns))
				{
					query = query
						.Where(t => t.Namespace!.Name == ns);
				}
				else
				{
					var n = nspace.ToString();
					query = query
						.Where(t => t.Namespace!.Name == n);
				}

				if (!string.IsNullOrWhiteSpace(name))
				{
					query = query
						.Where(t => t.Name.StartsWith(name));
				}

				break;
			}
			case 0:
			{
				var name = searchSpan[1..].Trim().ToString();
				query = query
					.Where(t => t.NamespaceId == null)
					.Where(t => t.Name.StartsWith(name));
				break;
			}
			case < 0:
			{
				var name = searchSpan.Trim().ToString();
				query = query
					.Where(t => t.Name.StartsWith(name));
				break;
			}
		}

		var tags = await query
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(tags);
	}
}