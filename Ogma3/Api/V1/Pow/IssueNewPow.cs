using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.PowService;

namespace Ogma3.Api.V1.Pow;

[Handler]
[MapGet("api/pow/issue")]
[UsedImplicitly]
public sealed partial class IssueNewPow(PowService powService, OgmaConfig config)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint) => endpoint.RequireRateLimiting(RateLimiting.PowIssue);

	[UsedImplicitly]
	public sealed record Query;

	private async ValueTask<Ok<Response>> Handle(Query _, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var pow = await powService.IssueChallenge();

		var res = new Response(pow.Token, pow.Difficulty, DateTimeOffset.UtcNow.AddSeconds(config.PowExpirySeconds));

		return TypedResults.Ok(res);
	}

	public sealed record Response(string Token, int Difficulty, DateTimeOffset ExpiresAt);
}