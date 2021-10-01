using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ogma3.Data;
using Ogma3.Infrastructure.Formatters;

namespace Ogma3.Services.RssService;

public class RssService : IRssService
{
    private readonly ApplicationDbContext _context;
    private readonly IUrlHelper _urlHelper;
        
    private readonly string _domain;

    public RssService(ApplicationDbContext context, IConfiguration config, IUrlHelperFactory urlHelperFactory,
        IActionContextAccessor actionContextAccessor)
    {
        _context = context;

        _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        _domain = $"https://{config.GetValue<string>("Domain")}";
    }

    public async Task<RssResult> GetStoriesAsync()
    {
        var stories = await _context.Stories
            .Where(s => !s.Rating.BlacklistedByDefault)
            .Where(s => s.PublicationDate != null)
            .Select(s => new SyndicationItem(
                s.Title,
                s.Hook,
                new Uri(_domain + _urlHelper.Page("/Story", new { s.Id, s.Slug })),
                s.Slug,
                s.PublicationDate ?? s.CreationDate
            ))
            .ToArrayAsync();

        return new RssResult
        {
            Title = "Genfic Stories RSS",
            Description = "Most recent stories published on Genfic",
            Items = stories,
        };
    }

    public async Task<RssResult> GetBlogpostsAsync()
    {
        var blogposts = await _context.Blogposts
            .Select(b => new SyndicationItem(
                b.Title,
                b.Body.Substring(0, 250),
                new Uri(_domain + _urlHelper.Page("/Blog/Post", new { b.Id, b.Slug })),
                b.Slug,
                b.PublicationDate ?? b.CreationDate
            ))
            .ToArrayAsync();

        return new RssResult
        {
            Title = "Genfic Blogposts RSS",
            Description = "Most recent blogposts published on Genfic",
            Items = blogposts,
        };
    }
}