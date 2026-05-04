using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ogma3.Api.V1;

using ReturnType = Results<Ok<Dictionary<string, string?>>, NotFound>;

[Handler]
[MapGet("api/test-three")]
public static partial class TestThree
{
	public sealed record Query();

	private static async ValueTask<ReturnType> Handle(Query q, IConfiguration cfg, CancellationToken _)
	{
		var secrets = cfg.AsEnumerable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		return TypedResults.Ok(secrets);
	}
}