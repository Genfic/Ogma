using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<CreatedAtRoute<ShelfDto>, UnauthorizedHttpResult>;

[Handler]
[MapPost("api/shelves")]
[Authorize]
public static partial class CreateShelf
{
	public sealed record Command
	(
		string Name,
		string Description,
		bool IsQuickAdd,
		bool IsPublic,
		bool TrackUpdates,
		string Color,
		long Icon
	);

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(s => s.Name)
				.MaximumLength(CTConfig.CShelf.MaxNameLength)
				.MinimumLength(CTConfig.CShelf.MinNameLength);
			RuleFor(s => s.Description)
				.MaximumLength(CTConfig.CShelf.MaxDescriptionLength);
			RuleFor(s => s.Color)
				.Length(7);
		}
	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var shelf = new Shelf
		{
			Name = request.Name,
			Description = request.Description,
			IsQuickAdd = request.IsQuickAdd,
			IsPublic = request.IsPublic,
			TrackUpdates = request.TrackUpdates,
			Color = request.Color,
			IconId = request.Icon,
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