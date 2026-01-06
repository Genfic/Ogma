using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public sealed partial class TestController
{
	public record Query(string Q);

	private async ValueTask<Ok> HandleAsync(Query _, CancellationToken ct)
	{
		await Task.Delay(100, ct);
		return TypedResults.Ok();
	}
}