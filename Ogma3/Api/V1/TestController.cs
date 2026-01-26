using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public sealed partial class TestController(IFusionCache cache)
{
	public sealed record Query(string Q);

	private async ValueTask<Ok> HandleAsync(Query _, CancellationToken ct)
	{
		await cache.ClearAsync(false, token: ct);

		return TypedResults.Ok();
	}
}