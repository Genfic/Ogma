using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Blog;

public class IndexModel(ApplicationDbContext context, OgmaConfig config) : PageModel
{
	public required IList<BlogpostCard> Posts { get; set; }
	public required string SearchBy { get; set; }
	public required EBlogpostSortingOptions SortBy { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<ActionResult> OnGetAsync([FromQuery] string q, [FromQuery] EBlogpostSortingOptions sort, [FromQuery] int page = 1)
	{
		SearchBy = q;
		SortBy = sort;

		var query = context.Blogposts.AsQueryable();

		if (!string.IsNullOrEmpty(q))
		{
			// Search by tags
			var splitQuery = q.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

			var tags = splitQuery
				.Where(x => x.StartsWith('#'))
				.Select(x => x.ToLower().Trim('#'))
				.ToArray();
			if (tags.Length > 0)
			{
				query = query
					.TagWith("Searching for blogposts with tags")
					.Where(b => tags.Any(i => b.Hashtags.Contains(i)));
			}

			// Search in title
			var search = splitQuery
				.Where(x => !x.StartsWith('#') && !string.IsNullOrEmpty(x))
				.ToArray();
			if (search.Length > 0)
			{
				query = query
					.TagWith("Searching for blogposts with title")
					.Where(b => EF.Functions.Like(b.Title.ToUpper(), $"%{string.Join(' ', search)}%".ToUpper()));
			}
		}

		// Save post count at this stage
		var postsCount = await query.CountAsync();

		// Sort
		query = sort switch
		{
			EBlogpostSortingOptions.TitleAscending => query.OrderBy(s => s.Title),
			EBlogpostSortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
			EBlogpostSortingOptions.DateAscending => query.OrderBy(s => s.PublicationDate),
			EBlogpostSortingOptions.DateDescending => query.OrderByDescending(s => s.PublicationDate),
			EBlogpostSortingOptions.WordsAscending => query.OrderBy(s => s.WordCount),
			EBlogpostSortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
			_ => query.OrderByDescending(s => s.WordCount)
		};

		// Finalize query
		Posts = await query
			.Where(b => b.PublicationDate != null)
			.Where(b => b.ContentBlockId == null)
			.Paginate(page, config.BlogpostsPerPage)
			.Select(b => new BlogpostCard
			{
				Id = b.Id,
				Title = b.Title,
				Slug = b.Slug,
				Body = b.Body,
				WordCount = b.WordCount,
				PublicationDate = b.PublicationDate,
				AuthorUserName = b.Author.UserName!,
				Hashtags = b.Hashtags
			})
			.ToListAsync();

		// Prepare pagination model
		Pagination = new Pagination
		{
			PerPage = config.BlogpostsPerPage,
			ItemCount = postsCount,
			CurrentPage = page
		};

		return Page();
	}
}