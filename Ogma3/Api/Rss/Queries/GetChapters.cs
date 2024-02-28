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

public static class GetChapters
{
	public sealed record Query(long StoryId) : IRequest<ActionResult<RssResult>>;

	public class Handler(ApplicationDbContext context, LinkGenerator generator, IHttpContextAccessor contextAccessor)
		: BaseHandler, IRequestHandler<Query, ActionResult<RssResult>>
	{
		public async ValueTask<ActionResult<RssResult>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (contextAccessor.HttpContext is not { } httpContext) return ServerError();

			var storyResult = await context.Stories
				.Where(s => !s.Rating.BlacklistedByDefault)
				.Where(s => s.PublicationDate != null)
				.Where(s => s.Id == request.StoryId)
				.Select(s => new
				{
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
						})
				})
				.FirstOrDefaultAsync(cancellationToken);

			if (storyResult is not { } story) return NotFound();

			var items = story.Chapters.Select(s => new SyndicationItem(
				s.Title,
				s.Hook,
				new Uri(generator.GetUriByPage(httpContext, "/Chapter", values: new { s.Id, s.Slug }) ?? ""),
				s.Slug,
				s.PublicationDate ?? default
			));

			return Ok(new RssResult
			{
				Title = $"{story.Title} — chapters",
				Description = $"All {story.ChapterCount} chapters of story {story.Title}",
				Items = items,
			});
		}
	}
}