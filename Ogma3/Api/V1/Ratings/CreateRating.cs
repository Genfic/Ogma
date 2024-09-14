using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.FileUploader;

namespace Ogma3.Api.V1.Ratings;

[Handler]
[MapPost("api/ratings")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateRating
{
	public sealed record Command(string Name, string Description, bool BlacklistedByDefault, byte Order, IFormFile Icon);

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
	
	private static async ValueTask<CreatedAtRoute<RatingApiDto>> HandleAsync(
		[FromForm] Command request,
		ApplicationDbContext context,
		ImageUploader uploader,
		OgmaConfig ogmaConfig,
		CancellationToken cancellationToken
	)
	{
		var rating = new Rating
		{
			Name = request.Name,
			Description = request.Description,
			BlacklistedByDefault = request.BlacklistedByDefault,
			Order = request.Order,
		};

		if (request.Icon is { Length: > 0 })
		{
			var fileData = await uploader.Upload(request.Icon, "ratings");
			rating.Icon = Path.Join(ogmaConfig.Cdn, fileData.Path);
			rating.IconId = fileData.FileId;
		}

		context.Ratings.Add(rating);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.CreatedAtRoute(rating.MapToApiDto(), nameof(GetRatingById), new GetRatingById.Query(rating.Id));
	}
}