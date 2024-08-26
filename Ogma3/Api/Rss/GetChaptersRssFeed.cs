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

using ReturnType = Results<RssResult, ServerError, NotFound>;

[Handler]
[MapGet("rss/story/{storyId:long}/chapters")]
public static partial class GetChaptersRssFeed
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.RequireRateLimiting(RateLimiting.Rss)
			.CacheOutput(CachePolicies.Rss)
			.WithName(nameof(GetChaptersRssFeed));

	[UsedImplicitly]
	public sealed record Query(long StoryId);

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		LinkGenerator generator,
		IHttpContextAccessor contextAccessor,
		IConfiguration config,
		CancellationToken cancellationToken
	)
	{
		if (contextAccessor.HttpContext is not {} httpContext) return ServerError.Instance();

		var storyResult = await context.Stories
			.Where(s => !s.Rating.BlacklistedByDefault)
			.Where(s => s.PublicationDate != null)
			.Where(s => s.Id == request.StoryId)
			.Select(s => new
			{
				Sid = s.Id,
				s.Title,
				s.ChapterCount,
				Chapters = s.Chapters
					.Where(c => c.PublicationDate != null)
					.OrderBy(c => c.PublicationDate)
					.Select(c => new
					{
						c.Id,
						c.Title,
						s.Hook,
						c.Slug,
						c.PublicationDate,
					}),
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (storyResult is null) return TypedResults.NotFound();

		var items = storyResult.Chapters.Select(s => new SyndicationItem(
			s.Title,
			s.Hook,
			new Uri(generator.GetUriByPage(httpContext, "/Chapter", values: new { storyResult.Sid, s.Id, s.Slug }) ?? ""),
			s.Slug,
			s.PublicationDate ?? default
		));

		return new RssResult(
			$"{storyResult.Title} — chapters", 
			$"All {storyResult.ChapterCount} chapters of story {storyResult.Title}",
			items, 
			$"https://{config.GetValue<string>("Domain")}"
		);
	}
}