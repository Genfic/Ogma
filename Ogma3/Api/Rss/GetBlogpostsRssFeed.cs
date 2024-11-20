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
[MapGet("rss/blogposts")]
public static partial class GetBlogpostsRssFeed
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.RequireRateLimiting(RateLimiting.Rss)
			.CacheOutput(CachePolicies.Rss)
			.ExcludeFromDescription()
			.WithName(nameof(GetBlogpostsRssFeed));

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

		var blogposts = await context.Blogposts
			.Select(b => new
			{
				b.Id,
				b.Title,
				Body = b.Body.Substring(0, 250),
				b.Slug,
				Date = b.PublicationDate ?? b.CreationDate,
			})
			.ToArrayAsync(cancellationToken);

		var items = blogposts.Select(b => new SyndicationItem(
			b.Title,
			b.Body,
			new Uri(generator.GetUriByPage(httpContext, "/Blog/Post", values: new { b.Id, b.Slug }) ?? ""),
			b.Slug,
			b.Date
		));

		return new RssResult(
			"Genfic Blogposts RSS",
			"Most recent blogposts published on Genfic",
			items,
			$"https://{config.GetValue<string>("Domain")}"
		);
	}
}