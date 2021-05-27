using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;

namespace Ogma3.Api.Rss
{
    [Route("rss", Name = nameof(RssController))]
    [ApiController]
    public class RssController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RssController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ResponseCache(Duration = 1200)]
        [HttpGet]
        public async Task<IActionResult> Stories()
        {
            var stories = await _context.Stories
                .Select(s => new StoryRssDto(
                    s.Title,
                    s.Slug,
                    s.Hook,
                    s.Slug,
                    s.ReleaseDate
                ))
                .ToListAsync();
            var feed = new SyndicationFeed("GenficRSS", "Desc", new Uri("https://genfic.net"), "RssUrl", DateTime.Now)
            {
                Copyright = new TextSyndicationContent($"2020 — {DateTime.Now.Year} Genfic"),
                Items = stories.Select(s => new SyndicationItem(
                    s.Title,
                    s.Description,
                    new Uri($"https://genfic.net/{s.Url}"),
                    s.Slug,
                    s.Date
                ))
            };

            await using var stream = new MemoryStream();
            await using var writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
                Indent = true,
                Async = true
            });
            new Rss20FeedFormatter(feed, false).WriteTo(writer);
            await writer.FlushAsync();
            return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
        }

        private record StoryRssDto(string Title, string Url, string Description, string Slug, DateTime Date);
    }
}