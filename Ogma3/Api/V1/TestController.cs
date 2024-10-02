using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Infrastructure.IResults;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public static partial class TestController
{
	[UsedImplicitly]
	public sealed record Query;
	
	private static async ValueTask<Results<ServerError, Ok>> HandleAsync(Query _, CancellationToken ct)
	{
		await Task.Delay(100, ct);
		return Random.Shared.Next() > int.MaxValue / 2 ? ServerError.Instance() : TypedResults.Ok();
	}
}