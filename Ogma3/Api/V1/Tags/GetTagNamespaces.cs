using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("tags/namespaces")]
public sealed partial class GetTagNamespaces(AppDbContext context)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetTagNamespaces));

	private async ValueTask<Ok<NamespaceDto[]>> Handle(Query _, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var values = await context.TagNamespaces
			.Select(v => new NamespaceDto(v.Id, v.Name))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(values);
	}

	[UsedImplicitly]
	public sealed record Query;

	public sealed record NamespaceDto(long Value, string Name);
}