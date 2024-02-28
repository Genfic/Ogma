using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Formatters;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.Rss.Queries;

public static class GetStories
{
	public sealed record Query : IRequest<ActionResult<RssResult>>;

	public class Handler(ApplicationDbContext context, LinkGenerator generator, IHttpContextAccessor contextAccessor)
		: BaseHandler, IRequestHandler<Query, ActionResult<RssResult>>
	{
		public async ValueTask<ActionResult<RssResult>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (contextAccessor.HttpContext is not { } httpContext) return ServerError();

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
					Date = s.PublicationDate ?? s.CreationDate
				})
				.ToArrayAsync(cancellationToken);

			var items = stories.Select(s => new SyndicationItem(
				s.Title,
				s.Hook,
				new Uri(generator.GetUriByPage(httpContext, "/Story", values: new { s.Id, s.Slug }) ?? ""),
				s.Slug,
				s.Date
			));

			return Ok(new RssResult
			{
				Title = "Genfic Stories RSS",
				Description = "50 most recent stories published on Genfic",
				Items = items,
			});
		}
	}
}