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
public class CacheController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheController> _logger;
    public CacheController(IMemoryCache cache, ILogger<CacheController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetCache()
    {
        if (_cache is MemoryCache mc)
        {
            return Ok(mc.Count);
        }

        return new ServerErrorResult("Could not count cache elements!");
    }

    [HttpDelete]
    [IgnoreAntiforgeryToken]
    public IActionResult DeleteCache()
    {
        _logger.LogWarning("Purging all caches...");
        
        if (_cache is MemoryCache mc)
        {
            mc.Compact(1.0);
            _logger.LogWarning("Cache purged!");
            return Ok("Cache purged!");
        }

        _logger.LogWarning("Could not purge cache!");
        return new ServerErrorResult("Could not purge cache!");

    }
}