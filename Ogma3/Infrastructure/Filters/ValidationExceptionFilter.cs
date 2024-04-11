using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ogma3.Infrastructure.ActionResults;

namespace Ogma3.Infrastructure.Filters;

[UsedImplicitly]
public class ValidationExceptionFilter : IExceptionFilter
{
	public void OnException(ExceptionContext context)
	{
		if (context.Exception is not ValidationException ex) return;

		var errors = ex.Errors
			.GroupBy(e => e.PropertyName)
			.ToDictionary(
				e => e.First().PropertyName,
				e => e.Select(v => v.ErrorMessage).ToHashSet().ToArray()
			);
		
		context.Result = new ProblemResult(new ValidationProblemDetails(errors)
		{
			Status = StatusCodes.Status422UnprocessableEntity,
			Title = "Validation error",
			Detail = "Data sent did not pass the validation process"
		});
	}
}