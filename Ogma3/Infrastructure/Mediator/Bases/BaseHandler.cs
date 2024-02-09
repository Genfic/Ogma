using Microsoft.AspNetCore.Mvc;
using Ogma3.Infrastructure.ActionResults;

namespace Ogma3.Infrastructure.Mediator.Bases;

public abstract class BaseHandler
{
	protected OkResult Ok() => new();
	protected OkObjectResult Ok(object o) => new(o);
	protected NotFoundResult NotFound() => new();
	protected NotFoundObjectResult NotFound(object o) => new(o);
	protected UnauthorizedResult Unauthorized() => new();
	protected UnauthorizedObjectResult Unauthorized(object o) => new(o);
	protected ServerErrorResult ServerError() => new();
	protected ServerErrorObjectResult ServerError(object o) => new(o);
	protected ConflictResult Conflict() => new();
	protected ConflictObjectResult Conflict(object o) => new(o);
	protected BadRequestResult BadRequest() => new();
	protected BadRequestObjectResult BadRequest(object o) => new(o);
	protected UnprocessableEntityResult UnprocessableEntity() => new();
	protected UnprocessableEntityObjectResult UnprocessableEntity(object o) => new(o);

	protected CreatedAtActionResult CreatedAtAction(
		string? actionName,
		string? controllerName,
		object? routeValues,
		object? value
	) => new(actionName, controllerName, routeValues, value);
}