using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ogma3.Infrastructure.ActionResults;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Areas.Admin.Api.V1.Cache;

[Route("admin/api/[controller]", Name = nameof(CacheController))]
[ApiController]
[Authorize(Roles = RoleNames.Admin)]
public class CacheController(IMemoryCache cache, ILogger<CacheController> logger) : ControllerBase
{
	[HttpGet]
	public ActionResult<int> GetCache()
	{
		if (cache is MemoryCache mc)
		{
			return Ok(mc.Count);
		}

		return new ServerErrorObjectResult("Could not count cache elements!");
	}

	[HttpDelete]
	[IgnoreAntiforgeryToken]
	public ActionResult<string> DeleteCache()
	{
		logger.LogWarning("Purging all caches...");

		if (cache is MemoryCache mc)
		{
			mc.Compact(1.0);
			logger.LogWarning("Cache purged!");
			return Ok("Cache purged!");
		}

		logger.LogWarning("Could not purge cache!");
		return new ServerErrorObjectResult("Could not purge cache!");
	}
}