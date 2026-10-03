using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Data.Reports;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.ServiceRegistrations;
using SafeRouting;
using Sqids;

namespace Ogma3.Areas.Admin.Api;

using Pages = Routes.Pages;

using ReturnType = Results<RedirectHttpResult, NotFound>;

[Handler]
[MapGet("admin/content/{kind}/{id}")]
[Authorize(AuthorizationPolicies.RequireStaffRole)]
public sealed partial class ReportableContentRedirector(AppDbContext context, SqidsEncoder<long> sqids, LinkGenerator linkGenerator)
{
	public sealed record Query(ReportableContentType Kind, long Id);

	private async ValueTask<ReturnType> HandleAsync(Query query, CancellationToken ct)
	{
		var route = query.Kind switch
		{
			ReportableContentType.Comment => await CommentRoute(query.Id, ct),
			ReportableContentType.User => await UserRoute(query.Id, ct),
			ReportableContentType.Story => await StoryRoute(query.Id, ct),
			ReportableContentType.Chapter => await ChapterRoute(query.Id, ct),
			ReportableContentType.Blogpost => await BlogpostRoute(query.Id, ct),
			ReportableContentType.Club => await ClubRoute(query.Id, ct),
			_ => throw new UnexpectedEnumValueException<ReportableContentType>(query.Kind),
		};

		if (route is null)
		{
			return TypedResults.NotFound();
		}

		var fragment = query.Kind is ReportableContentType.Comment
			? new FragmentString($"#comment-{sqids.Encode(query.Id)}")
			: default;

		var path = route.Path(linkGenerator, fragment: fragment);

		return TypedResults.Redirect(path);
	}

	private async Task<IPageRouteValues?> UserRoute(long id, CancellationToken ct)
	{
		var userName = await context.Users
			.Where(u => u.Id == id)
			.Select(u => u.UserName)
			.FirstOrDefaultAsync(ct);
		return userName is null
			? null
			: Pages.User_Index.Get(userName);
	}

	private async Task<IPageRouteValues?> StoryRoute(long id, CancellationToken ct)
	{
		var slug = await context.Stories
			.Where(s => s.Id == id)
			.Select(s => s.Slug)
			.FirstOrDefaultAsync(ct);
		return slug is null
			? null
			: Pages.Story.Get(id, slug);
	}

	private async Task<IPageRouteValues?> ChapterRoute(long id, CancellationToken ct)
	{
		var data = await context.Chapters
			.Where(c => c.Id == id)
			.Select(c => new { c.Slug, c.StoryId })
			.FirstOrDefaultAsync(ct);
		return data is null
			? null
			: Pages.Chapter.Get(data.StoryId, id, data.Slug);
	}

	private async Task<IPageRouteValues?> BlogpostRoute(long id, CancellationToken ct)
	{
		var slug = await context.Blogposts
			.Where(b => b.Id == id)
			.Select(b => b.Slug)
			.FirstOrDefaultAsync(ct);
		return slug is null
			? null
			: Pages.Blog_Post.Get(id, slug);
	}

	private async Task<IPageRouteValues?> ClubRoute(long id, CancellationToken ct)
	{
		var slug = await context.Clubs
			.Where(c => c.Id == id)
			.Select(c => c.Slug)
			.FirstOrDefaultAsync(ct);
		return slug is null
			? null
			: Pages.Club_Index.Get(id, slug);
	}

	private async Task<IPageRouteValues?> CommentRoute(long id, CancellationToken ct)
	{
		var threadMeta = await context.Comments
			.Where(c => c.Id == id)
			.Select(c => new
			{
				c.CommentThread.Source,
				c.CommentThread.SourceId,
			})
			.FirstOrDefaultAsync(ct);

		if (threadMeta is null)
		{
			return null;
		}

		return threadMeta.Source switch
		{
			CommentSource.Chapter => await ChapterRoute(threadMeta.SourceId, ct),
			CommentSource.Blogpost => await BlogpostRoute(threadMeta.SourceId, ct),
			CommentSource.Profile => await UserRoute(threadMeta.SourceId, ct),
			CommentSource.ForumPost => await ForumPostRoute(threadMeta.SourceId, ct),
			CommentSource.NewsPost => await NewsPostRoute(threadMeta.SourceId, ct),
			_ => throw new UnexpectedEnumValueException<CommentSource>(threadMeta.Source),
		};
	}

	private async Task<IPageRouteValues?> ForumPostRoute(long id, CancellationToken ct)
	{
		var clubId = await context.ClubThreads
			.Where(t => t.Id == id)
			.Select(t => (long?)t.ClubId)
			.FirstOrDefaultAsync(ct);
		return clubId is not {} cid
			? null
			: Pages.Club_Forums_Details.Get(id, cid);
	}

	private async Task<IPageRouteValues?> NewsPostRoute(long id, CancellationToken ct)
	{
		var slug = await context.News
			.Where(n => n.Id == id)
			.Select(n => n.Slug)
			.FirstOrDefaultAsync(ct);
		return slug is null
			? null
			: Pages.News_Post.Get(id, slug);
	}
}