#nullable enable

using Microsoft.AspNetCore.Mvc;
using Ogma3.Infrastructure.ActionResults;

namespace Ogma3.Infrastructure.MediatR.Bases;

public class BaseHandler
{
    public OkResult Ok() => new();
    public OkObjectResult Ok(object o) => new(o);
    public NotFoundResult NotFound() => new();
    public NotFoundObjectResult NotFound(object o) => new(o);
    public UnauthorizedResult Unauthorized() => new();
    public UnauthorizedObjectResult Unauthorized(object o) => new(o);
    public ServerErrorResult ServerError() => new();
    public ServerErrorObjectResult ServerError(object o) => new(o);
    public ConflictResult Conflict() => new();
    public ConflictObjectResult Conflict(object o) => new(o);
    public BadRequestResult BadRequest() => new();
    public BadRequestObjectResult BadRequest(object o) => new(o);
    public UnprocessableEntityResult UnprocessableEntity() => new();
    public UnprocessableEntityObjectResult UnprocessableEntity(object o) => new(o);

    public CreatedAtActionResult CreatedAtAction(
        string? actionName,
        string? controllerName,
        object? routeValues,
        object? value
    ) => new(actionName, controllerName, routeValues, value);
}