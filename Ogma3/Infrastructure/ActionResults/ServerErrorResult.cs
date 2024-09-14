using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Ogma3.Infrastructure.ActionResults;

[DefaultStatusCode(DefaultStatusCode)]
public sealed class ServerErrorResult : ObjectResult
{
	private const int DefaultStatusCode = StatusCodes.Status500InternalServerError;

	public ServerErrorResult() : base(DefaultStatusCode)
	{
		StatusCode = DefaultStatusCode;
	}
}