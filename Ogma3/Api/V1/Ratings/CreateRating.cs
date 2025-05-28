using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
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
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateRating
{
	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[MinLength(CTConfig.Rating.MinNameLength)]
		[MaxLength(CTConfig.Rating.MaxNameLength)]
		public required string Name { get; init; }
		[MinLength(CTConfig.Rating.MinDescriptionLength)]
		[MaxLength(CTConfig.Rating.MaxDescriptionLength)]
		public required string Description { get; init; }
		public required bool BlacklistedByDefault { get; init; }
		public required byte Order { get; init; }
		[FileExtension(".svg")]
		[FileSize(100 * 1024)]
		public required IFormFile Icon { get; init; }
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