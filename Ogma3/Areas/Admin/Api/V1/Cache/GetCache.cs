using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Infrastructure.IResults;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Api.V1.Cache;

using ReturnType = Results<Ok<int>, ServerError>;

[Handler]
[MapGet("admin/api/cache")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class GetCache
{
	[UsedImplicitly]
	public sealed record Query;

	private static async ValueTask<ReturnType> HandleAsync(Query _, IMemoryCache cache, CancellationToken __)
	{
		await Task.Yield();
		
		if (cache is MemoryCache mc)
		{
			return TypedResults.Ok(mc.Count);
		}

		return ServerError.Instance("Could not count cache elements!");
	}
}