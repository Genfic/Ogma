using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Ogma3.Infrastructure.ActionResults;

[DefaultStatusCode(DefaultStatusCode)]
public class ServerErrorObjectResult : ObjectResult
{
	private const int DefaultStatusCode = StatusCodes.Status500InternalServerError;

	public ServerErrorObjectResult(object value) : base(value)
	{
		StatusCode = DefaultStatusCode;
	}
}