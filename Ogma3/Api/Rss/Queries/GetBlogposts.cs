using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Formatters;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.Rss.Queries;

public static class GetBlogposts
{
	public sealed record Query : IRequest<ActionResult<RssResult>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<RssResult>>
	{
		private readonly ApplicationDbContext _context;
		private readonly LinkGenerator _generator;
		private readonly IHttpContextAccessor _contextAccessor;

		public Handler(ApplicationDbContext context, LinkGenerator generator, IHttpContextAccessor contextAccessor)
		{
			_context = context;
			_generator = generator;
			_contextAccessor = contextAccessor;
		}
		
		public async Task<ActionResult<RssResult>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (_contextAccessor.HttpContext is not { } httpContext) return ServerError();
			
			var blogposts = await _context.Blogposts
				.Select(b => new {
					b.Id,
					b.Title,
					Body = b.Body.Substring(0, 250),
					b.Slug,
					Date = b.PublicationDate ?? b.CreationDate
				})
				.ToArrayAsync(cancellationToken);

			var items = blogposts.Select(b => new SyndicationItem(
				b.Title,
				b.Body,
				new Uri(_generator.GetUriByPage(httpContext, "/Blog/Post", values: new { b.Id, b.Slug }) ?? ""),
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