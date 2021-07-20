using Microsoft.AspNetCore.Mvc;

namespace Ogma3.Infrastructure.MediatR
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> r) => r switch
        {
            { Status: ResultStatus.Ok, Data: { } data } => new OkObjectResult(data),
            { Status: ResultStatus.Conflict, Error: { } error } => new ConflictObjectResult(error),
            { Status: ResultStatus.Unauthorized, Error: { } error } => new UnauthorizedObjectResult(error),
            { Status: ResultStatus.NotFound, Error: { } error } => new NotFoundObjectResult(error),
            { Error: { } error } => new BadRequestObjectResult(error),
            _ => new StatusCodeResult(500)
        };
    }
}