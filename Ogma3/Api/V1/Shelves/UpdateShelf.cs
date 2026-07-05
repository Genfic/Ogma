using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<Ok, UnauthorizedHttpResult, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapPut("shelves")]
[Authorize]
public sealed partial class UpdateShelf(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var rows = await context.Shelves
			.Where(s => s.Id == request.Id)
			.Where(s => s.OwnerId == uid)
			.ExecuteUpdateAsync(setPropertyCalls: setters => setters
				.SetProperty(propertyExpression: s => s.Name, request.Name)
				.SetProperty(propertyExpression: s => s.Description, request.Description)
				.SetProperty(propertyExpression: s => s.IsQuickAdd, request.IsQuickAdd)
				.SetProperty(propertyExpression: s => s.IsPublic, request.IsPublic)
				.SetProperty(propertyExpression: s => s.TrackUpdates, request.TrackUpdates)
				.SetProperty(propertyExpression: s => s.Color, request.Color)
				.SetProperty(propertyExpression: s => s.IconId, request.IconId), cancellationToken);

		return rows > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command
	(
		long Id,
		[property: System.ComponentModel.DataAnnotations.MinLength(CTConfig.Shelf.MinNameLength)]
		[property: System.ComponentModel.DataAnnotations.MaxLength(CTConfig.Shelf.MaxNameLength)]
		string Name,
		[property: System.ComponentModel.DataAnnotations.MaxLength(CTConfig.Shelf.MaxDescriptionLength)]
		string Description,
		bool IsQuickAdd,
		bool IsPublic,
		bool TrackUpdates,
		[property: System.ComponentModel.DataAnnotations.MaxLength(7)]
		string Color,
		long? IconId
	) : IValidationTarget<Command>;
}