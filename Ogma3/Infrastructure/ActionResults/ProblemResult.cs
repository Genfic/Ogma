using Microsoft.AspNetCore.Mvc;

namespace Ogma3.Infrastructure.ActionResults;

public class ProblemResult : ObjectResult
{
	private const int DefaultStatusCode = StatusCodes.Status422UnprocessableEntity;

	public ProblemResult(object value) : base(value)
	{
		StatusCode = DefaultStatusCode;
	}
}