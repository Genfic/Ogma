using System.ServiceModel.Syndication;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.IResults;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.Rss;

using ReturnType = Results<RssResult, InternalServerError>;

[Handler]
[MapGet("rss/stories")]
public static partial class GetStoriesRssFeed
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.RequireRateLimiting(RateLimiting.Rss)
			.CacheOutput(CachePolicies.Rss)
			.WithName(nameof(GetStoriesRssFeed));

	[UsedImplicitly]
	public sealed record Query;

	private static async ValueTask<ReturnType> HandleAsync(
		Query _,
		ApplicationDbContext context,
		LinkGenerator generator,
		IHttpContextAccessor contextAccessor,
		IConfiguration config,
		CancellationToken cancellationToken
	)
	{
		if (contextAccessor.HttpContext is not {} httpContext) return TypedResults.InternalServerError();

		var stories = await context.Stories
			.Where(s => !s.Rating.BlacklistedByDefault)
			.Where(s => s.PublicationDate != null)
			.Take(50)
			.Select(s => new
			{
				s.Id,
				s.Title,
				s.Hook,
				s.Slug,
				Date = s.PublicationDate ?? s.CreationDate,
			})
			.ToArrayAsync(cancellationToken);

		var items = stories.Select(s => new SyndicationItem(
			s.Title,
			s.Hook,
			new Uri(generator.GetUriByPage(httpContext, "/Story", values: new { s.Id, s.Slug }) ?? ""),
			s.Slug,
			s.Date
		));

		return new RssResult(
			"Genfic Stories RSS",
			"50 most recent stories published on Genfic",
			items,
			$"https://{config.GetValue<string>("Domain")}"
		);
	}
}