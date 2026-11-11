using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.BlogpostSearchQueryParser;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Blog;

public sealed class IndexModel(AppDbContext context, OgmaConfig config) : PageModel
{
	public required IList<BlogpostCard> Posts { get; set; }
	public string? SearchBy { get; set; }
	public required EBlogpostSortingOptions SortBy { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<ActionResult> OnGetAsync([FromQuery] string? q = null, [FromQuery] EBlogpostSortingOptions? sort = null, [FromQuery] int page = 1)
	{
		SearchBy = q;
		SortBy = sort ?? EBlogpostSortingOptions.DateDescending;

		var query = context.Blogposts
			.Where(b => b.IsVisible)
			.Where(b => b.ContentBlockId == null);

		if (User.Identity?.IsAuthenticated is not true)
		{
			query = query.Where(b => b.IsLocked == false);
		}

		if (!string.IsNullOrWhiteSpace(q))
		{
			var tags = new HashSet<string>();
			foreach (var token in BlogpostSearchQueryParser.Parse(q))
			{
				query = token switch
				{
					BlogpostAuthorToken t => query
						.TagWith("Searching for blogposts by author")
						.Where(b => b.Author.NormalizedUserName.Contains(t.Value)),
					BlogpostTitleToken t => query
						.TagWith("Searching for blogposts with title")
						// ReSharper disable once EntityFramework.ClientSideDbFunctionCall
						.Where(b => EF.Functions.ILike(b.Title, $"%{t.Value}%")),
					BlogpostTagToken t => CollectTagToken(t, query, tags),
				};
			}

			if (tags.Count > 0)
			{
				query = query
					.TagWith("Searching for blogposts with tags")
					.Where(b => b.Hashtags.Intersect(tags).Any());
			}
		}

		// Save post-count at this stage
		var postsCount = await query.CountAsync();

		// Sort
		query = SortBy switch
		{
			EBlogpostSortingOptions.TitleAscending => query.OrderBy(s => s.Title),
			EBlogpostSortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
			EBlogpostSortingOptions.DateAscending => query.OrderBy(s => s.PublicationDate),
			EBlogpostSortingOptions.DateDescending => query.OrderByDescending(s => s.PublicationDate),
			EBlogpostSortingOptions.WordsAscending => query.OrderBy(s => s.WordCount),
			EBlogpostSortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
			_ => query.OrderByDescending(s => s.PublicationDate),
		};

		// Finalize query
		Posts = await query
			.Paginate(page, config.BlogpostsPerPage)
			.Select(b => new BlogpostCard
			{
				Id = b.Id,
				Title = b.Title,
				Slug = b.Slug,
				Body = b.Body.Substring(0, b.ExcerptCutoff),
				WordCount = b.WordCount,
				PublicationDate = b.PublicationDate,
				AuthorUserName = b.Author.UserName,
				Hashtags = b.Hashtags,
			})
			.ToListAsync();

		// Prepare the pagination model
		Pagination = new Pagination
		{
			PerPage = config.BlogpostsPerPage,
			ItemCount = postsCount,
			CurrentPage = page,
		};

		return Page();
	}

	private static IQueryable<Blogpost> CollectTagToken(BlogpostTagToken token, IQueryable<Blogpost> query, HashSet<string> tags)
	{
		tags.Add(token.Value);
		return query;
	}
}