using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Mediator;

namespace Ogma3.Infrastructure.Mediator.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	public ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
	{
		var failures = new List<ValidationFailure>();
		foreach (var validator in validators)
		{
			var result = validator.Validate(message);
			failures.AddRange(result.Errors);
		}

		if (failures.Count != 0)
		{
			throw new ValidationException(failures);
		}

		return next(message, cancellationToken);
	}
}