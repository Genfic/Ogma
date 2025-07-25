using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data.Tags;

namespace Ogma3.Api.V1.Tags;

[Handler]
[MapGet("api/tags/namespaces")]
public static partial class GetTagNamespaces
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetTagNamespaces));

	public sealed record Query;

	private static ValueTask<Ok<NamespaceDto[]>> HandleAsync(
		Query _,
		CancellationToken __
	)
	{
		var values = ETagNamespaceExtensions.GetValues()
			.Select(v => new NamespaceDto ((int)v, v.ToStringFast()))
			.ToArray();

		return ValueTask.FromResult(TypedResults.Ok(values));
	}

	public sealed record NamespaceDto(int Value, string Name);
}