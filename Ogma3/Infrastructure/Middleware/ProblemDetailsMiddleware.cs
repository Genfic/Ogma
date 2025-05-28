using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Ogma3.Infrastructure.Middleware;

public static class ProblemDetailsMiddleware
{
	public static void ConfigureProblemDetails(ProblemDetailsOptions options) =>
		options.CustomizeProblemDetails = c =>
		{
			if (c.Exception is null)
			{
				return;
			}

			c.ProblemDetails = c.Exception switch
			{
				ValidationException ex => new ValidationProblemDetails(
					ex
						.Errors
						.GroupBy(x => x.PropertyName, StringComparer.OrdinalIgnoreCase)
						.ToDictionary(
							x => x.Key,
							x => x.Select(e => e.ErrorMessage).ToArray(),
							StringComparer.OrdinalIgnoreCase
						)
				)
				{
					Status = StatusCodes.Status400BadRequest,
				},

				// other exception handling as desired

				var ex => new ProblemDetails
				{
					Detail = "An error has occurred.",
					Status = StatusCodes.Status500InternalServerError,
				},
			};

			c.HttpContext.Response.StatusCode =
				c.ProblemDetails.Status
				?? StatusCodes.Status500InternalServerError;
		};
}