using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ogma3.Infrastructure.Exceptions;

namespace Ogma3.Infrastructure.Filters;

[UsedImplicitly]
public sealed class AuthenticationExceptionFilter : IExceptionFilter
{
	public void OnException(ExceptionContext context)
	{
		if (context.Exception is not NotAuthenticatedException)
		{
			return;
		}

		context.Result = new UnauthorizedResult();
		context.ExceptionHandled = true;
	}
}