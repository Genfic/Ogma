using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ConfigTestController))]
    [ApiController]
    public class ConfigTestController : Controller
    {
        private readonly OgmaConfig _ogmaConfig;

        public ConfigTestController(OgmaConfig ogmaConfig)
        {
            _ogmaConfig = ogmaConfig;
        }

        // GET
        [HttpGet]
        public OgmaConfig Get()
        {
            return _ogmaConfig;
        }

        [HttpPost]
        public async Task<OgmaConfig> Post()
        {
            var r = new Random();

            var len = r.Next(1, 10);
            var arr = new double[len];
            for(var i = 0; i < len; i++) {
                arr[i] = r.NextDouble() * 100;	
            }
            await _ogmaConfig.PersistAsync();
            
            return _ogmaConfig;
        }
    }
}