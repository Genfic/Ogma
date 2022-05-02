using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Ogma3.Infrastructure.MediatR.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;
	public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

	public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
	{
		var failures = _validators
			.Select(v => v.Validate(request))
			.SelectMany(result => result.Errors)
			.Where(f => f is not null)
			.ToList();

		if (failures.Any())
		{
			throw new ValidationException(failures);
		}

		return next();
	}
}