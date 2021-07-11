using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ogma3.Data;
using Ogma3.Services;
using Ogma3.Services.Mailer;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(TestController))]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<MailGunOptions> _options;

        public TestController(IHttpClientFactory httpClientFactory, IOptions<MailGunOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        // GET
        [HttpGet]
        public async Task GetTestAsync()
        {
            Console.WriteLine("Starting");
            var mailer = new MailGunMailer(_httpClientFactory, _options);
            await mailer.SendEmailAsync("admin@genfic.net", "Hello", "Did it work?");
            Console.WriteLine("Sent");
        }
    }
}