using System.ComponentModel.DataAnnotations;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<Ok, UnauthorizedHttpResult, NotFound>;

[Handler]
[MapPut("api/shelves")]
[Authorize]
public static partial class UpdateShelf
{
	public sealed record Command
	(
		long Id,
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
		long Icon
	);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var rows = await context.Shelves
			.Where(s => s.Id == request.Id)
			.Where(s => s.OwnerId == uid)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(s => s.Name, request.Name)
				.SetProperty(s => s.Description, request.Description)
				.SetProperty(s => s.IsQuickAdd, request.IsQuickAdd)
				.SetProperty(s => s.IsPublic, request.IsPublic)
				.SetProperty(s => s.TrackUpdates, request.TrackUpdates)
				.SetProperty(s => s.Color, request.Color)
				.SetProperty(s => s.IconId, request.Icon), cancellationToken: cancellationToken);

		return rows > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}