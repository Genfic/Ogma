using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ogma3.Infrastructure.MediatR.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var failures = new List<ValidationFailure>();
		foreach (var validator in validators)
		{
			var result = validator.Validate(request);
			failures.AddRange(result.Errors);
		}

		if (failures.Count != 0)
		{
			throw new ValidationException(failures);
		}

		return next();
	}
}