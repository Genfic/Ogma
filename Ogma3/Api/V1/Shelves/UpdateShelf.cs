using FluentValidation;
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
		string Name,
		string Description,
		bool IsQuickAdd,
		bool IsPublic,
		bool TrackUpdates,
		string Color,
		long Icon
	);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(s => s.Id)
				.NotEmpty();
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