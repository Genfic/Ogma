using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.FileUploader;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Results<NotFound, CreatedAtRoute<RatingApiDto>>;

[Handler]
[MapPut("api/ratings")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateRating
{
	public sealed record Command(long Id, string Name, string Description, bool BlacklistedByDefault, byte Order, IFormFile Icon);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(r => r.Name)
				.MinimumLength(CTConfig.CRating.MinNameLength)
				.MaximumLength(CTConfig.CRating.MaxNameLength);
			RuleFor(r => r.Description)
				.MinimumLength(CTConfig.CRating.MinDescriptionLength)
				.MaximumLength(CTConfig.CRating.MaxDescriptionLength);
			RuleFor(r => r.Icon)
				.FileHasExtension(".svg")
				.FileSmallerThan(100 * 1024);
		}
	}

	private static async ValueTask<ReturnType> HandleAsync(
		[FromForm] Command request,
		ApplicationDbContext context,
		ImageUploader uploader,
		OgmaConfig ogmaConfig,
		CancellationToken cancellationToken
	)
	{
		var rating = await context.Ratings
			.Where(r => r.Id == request.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (rating is null) return TypedResults.NotFound();

		rating.Name = request.Name;
		rating.Description = request.Description;
		rating.BlacklistedByDefault = request.BlacklistedByDefault;
		rating.Order = request.Order;

		if (request.Icon is { Length: > 0 })
		{
			if (!string.Equals(request.Name, rating.Name, StringComparison.OrdinalIgnoreCase))
			{
				await uploader.Delete(rating.Icon, rating.IconId, cancellationToken);
			}

			var fileData = await uploader.Upload(request.Icon, "ratings");
			rating.Icon = Path.Join(ogmaConfig.Cdn, fileData.Path);
			rating.IconId = fileData.FileId;
		}

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.CreatedAtRoute(rating.MapToApiDto(), nameof(GetRatingById), new GetRatingById.Query(rating.Id));
	}
}