using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ogma3.Infrastructure.Logging.OperationTiming;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1;

[Route("api/[controller]", Name = nameof(TestController))]
[ApiController]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public class TestController(ILogger<TestController> logger) : ControllerBase
{

	// GET
	[HttpGet]
	public async Task GetTestAsync()
	{
		const string who = "mom";
		logger.LogInformation("Test action start");
		using (logger.TimeOperation("Doing your {Who}", who))
		{
			await Task.Delay(500);
		}
		logger.LogInformation("Test action end");
	}

}