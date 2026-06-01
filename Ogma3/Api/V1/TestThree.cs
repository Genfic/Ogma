using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ogma3.Api.V1;

using ReturnType = Results<Ok<Dictionary<string, string?>>, NotFound>;

[Handler]
[MapGet("api/test-three")]
public sealed partial class TestThree(IConfiguration cfg)
{

	private async ValueTask<ReturnType> Handle(Query q, CancellationToken _)
	{
		await Task.CompletedTask;
	#if DEBUG
		var secrets = cfg.AsEnumerable().ToDictionary(keySelector: kvp => kvp.Key, elementSelector: kvp => kvp.Value);
		return TypedResults.Ok(secrets);
	#else
		return TypedResults.NotFound();
	#endif
	}

	public sealed record Query;
}