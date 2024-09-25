using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.FileUploader;

namespace Ogma3.Api.V1.Ratings;

using ReturnType = Results<Ok<long>, NotFound>;

[Handler]
[MapDelete("api/ratings/{ratingId:long}")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class DeleteRating
{
	public sealed record Command(long RatingId);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		ImageUploader uploader,
		CancellationToken cancellationToken
	)
	{
		var rating = await context.Ratings
			.Where(r => r.Id == request.RatingId)
			.FirstOrDefaultAsync(cancellationToken);

		if (rating is null) return TypedResults.NotFound();

		context.Ratings.Remove(rating);

		if (rating is { Icon: not null, IconId: not null })
		{
			await uploader.Delete(rating.Icon, rating.IconId, cancellationToken);
		}

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.Ok(rating.Id);
	}
}