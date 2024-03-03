using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Formatters;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.Rss.Queries;

public static class GetBlogposts
{
	public sealed record Query : IRequest<ActionResult<RssResult>>;

	[UsedImplicitly]
	public class Handler(ApplicationDbContext context, LinkGenerator generator, IHttpContextAccessor contextAccessor)
		: BaseHandler, IRequestHandler<Query, ActionResult<RssResult>>
	{
		public async ValueTask<ActionResult<RssResult>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (contextAccessor.HttpContext is not { } httpContext) return ServerError();

			var blogposts = await context.Blogposts
				.Select(b => new {
					b.Id,
					b.Title,
					Body = b.Body.Substring(0, 250),
					b.Slug,
					Date = b.PublicationDate ?? b.CreationDate,
					a ="a"
				})
				.ToArrayAsync(cancellationToken);

			var items = blogposts.Select(b => new SyndicationItem(
				b.Title,
				b.Body,
				new Uri(generator.GetUriByPage(httpContext, "/Blog/Post", values: new { b.Id, b.Slug }) ?? ""),
				b.Slug,
				b.Date
			));

			return Ok(new RssResult
			{
				Title = "Genfic Blogposts RSS",
				Description = "Most recent blogposts published on Genfic",
				Items = items,
			});
		}
	}
}