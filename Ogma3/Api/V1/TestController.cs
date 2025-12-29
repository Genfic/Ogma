using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;

namespace Ogma3.Api.V1;

[Handler]
[MapGet("api/test")]
public sealed partial class TestController(ApplicationDbContext context)
{
	public record Query(string Q);

	private async ValueTask<Ok> HandleAsync(Query q, CancellationToken ct)
	{
		await Task.CompletedTask;
		return TypedResults.Ok();
	}
}