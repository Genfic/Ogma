using FluentValidation;
using FluentValidation.Results;
using Immediate.Handlers.Shared;
using Ogma3.Infrastructure.Mediator.Behaviours;

[assembly: Behaviors(typeof(ValidationBehavior<,>))]

namespace Ogma3.Infrastructure.Mediator.Behaviours;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : Behavior<TRequest, TResponse>
{
	public override async ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
	{
		var failures = new List<ValidationFailure>();
		foreach (var validator in validators)
		{
			var result = await validator.ValidateAsync(request, cancellationToken);
			failures.AddRange(result.Errors);
		}

		if (failures.Count != 0)
		{
			throw new ValidationException(failures);
		}

		var response = await Next(request, cancellationToken);
		return response;
	}
}