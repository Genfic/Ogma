using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Infrastructure.ActionResults;
using Serilog;

namespace Ogma3.Infrastructure.MediatR.Behaviours
{
    public class ValidationBehaviour<TRequest> : IPipelineBehavior<TRequest, IActionResult>
    {
        private readonly IValidator<TRequest> _validator;
        public ValidationBehaviour(IValidator<TRequest> validator) => _validator = validator;

        public async Task<IActionResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<IActionResult> next)
        {
            Log.Information("Entered validation behaviour");
            
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (result.IsValid) return await next();

            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    e => e.First().PropertyName, 
                    e => e.Select(v => v.ErrorMessage).ToArray()
                );
            
            Log.Information("Validation errors are happening");
            
            return new ProblemResult(new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Validation error",
                Detail = "Data sent did not pass the validation process"
            });
        }
    }
}