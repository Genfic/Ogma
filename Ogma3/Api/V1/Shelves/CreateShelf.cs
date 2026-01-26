using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<CreatedAtRoute<ShelfDto>, UnauthorizedHttpResult>;

[Handler]
[MapPost("api/shelves")]
[Authorize]
public static partial class CreateShelf
{
	[Validate]
	public sealed partial record Command
	(
		[property: MinLength(CTConfig.Shelf.MinNameLength)]
		[property: MaxLength(CTConfig.Shelf.MaxNameLength)]
		string Name,
		[property: MaxLength(CTConfig.Shelf.MaxDescriptionLength)]
		string Description,
		bool IsQuickAdd,
		bool IsPublic,
		bool TrackUpdates,
		[property: MaxLength(7)]
		string Color,
		long IconId
	) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var shelf = new Shelf
		{
			Name = request.Name,
			Description = request.Description,
			IsQuickAdd = request.IsQuickAdd,
			IsPublic = request.IsPublic,
			TrackUpdates = request.TrackUpdates,
			Color = request.Color,
			IconId = request.IconId,
			OwnerId = uid,
		};

		context.Shelves.Add(shelf);
		await context.SaveChangesAsync(cancellationToken);

		var dto = new ShelfDto
		{
			Id = shelf.Id,
			Name = shelf.Name,
			Description = shelf.Description,
			IsDefault = shelf.IsDefault,
			IsPublic = shelf.IsPublic,
			IsQuickAdd = shelf.IsQuickAdd,
			TrackUpdates = shelf.TrackUpdates,
			StoriesCount = 0,
			IconName = shelf.Icon?.Name,
			IconId = 0,
		};

		return TypedResults.CreatedAtRoute(dto, nameof(GetShelf), new GetShelf.Query(shelf.Id));
	}
}