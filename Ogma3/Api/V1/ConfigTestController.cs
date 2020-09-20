using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Services.SiteConfig;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ConfigTestController))]
    [ApiController]
    public class ConfigTestController : Controller
    {
        private readonly ISiteConfig _config;

        public ConfigTestController(ISiteConfig config)
        {
            _config = config;
        }

        // GET
        [HttpGet]
        public ActionResult<string> Get([FromQuery]string key)
        {
            return _config.GetValue<string>(key);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post(PostData data)
        {
            _config.SetValue(data.Key, data.Val);
            await _config.PersistAsync();
            return _config.GetValue<string>(data.Key);
        }

        [HttpPost("typed")]
        public async Task<int> Post(TypedPostData data)
        {
            _config.SetValue(data.Key, data.Val);
            await _config.PersistAsync();
            return _config.GetValue<int>(data.Key);
        }
        
        public class PostData
        {
            public string Key { get; set; }
            public string Val { get; set; }
        }
        
        public class TypedPostData
        {
            public string Key { get; set; }
            public int Val { get; set; }
        }
    }
}