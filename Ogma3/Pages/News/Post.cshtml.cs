using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Pages.Shared;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Pages.News;

public sealed class DetailsModel(AppDbContext context) : PageModel
{
	public required NewsDetails NewsPost { get; set; }
	public required CommentsThreadDto? CommentsThread { get; set; }

	public async Task<IActionResult> OnGetAsync(long id, string? slug = null)
	{
		var post = await context.News
			.TagWith($"Get news -> {id}")
			.Where(b => b.Id == id)
			.Where(b => b.IsVisible)
			.Where(b => b.PublicationDate != null)
			.ProjectToDetails()
			.FirstOrDefaultAsync();

		if (post is null) return NotFound();

		NewsPost = post;

		CommentsThread = await context.CommentThreads
			.Where(t => t.NewsId == post.Id)
			.Select(t => new CommentsThreadDto
			{
				Id = t.Id,
				LockDate = t.LockDate,
				Type = CommentSource.NewsPost,
			})
			.FirstOrDefaultAsync();

		return Page();
	}
}

public sealed record NewsDetails
(
	long Id,
	string Title,
	string Slug,
	string Body,
	DateTimeOffset PublicationDate,
	string AuthorUserName,
	string AuthorAvatarUrl
);

[Mapper]
public static partial class NewsMapper
{
	public static partial IQueryable<NewsDetails> ProjectToDetails(this IQueryable<Data.NewsPosts.News> query);
}