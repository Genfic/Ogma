using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Cache;

using ReturnType = Results<Ok, InternalServerError<string>>;

[Handler]
[MapDelete("admin/api/cache")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class PurgeCache
{
	public sealed record Query;

	// ReSharper disable once UnusedParameter.Local
	private static async ValueTask<ReturnType> HandleAsync(Query _, IMemoryCache cache, ILogger<Query> logger, CancellationToken __)
	{
		await Task.Yield();
		
		logger.LogWarning("Purging all caches...");

		if (cache is MemoryCache mc)
		{
			mc.Compact(1.0);
			logger.LogWarning("Cache purged!");
			return TypedResults.Ok();
		}

		logger.LogWarning("Could not purge cache!");
		return TypedResults.InternalServerError("Could not purge cache!");
	}
}