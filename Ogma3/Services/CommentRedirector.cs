using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Routes.Pages;

namespace Ogma3.Services;

public sealed class CommentRedirector(ApplicationDbContext context, LinkGenerator linkGenerator)
{
	public async Task<string?> RedirectToComment(long commentId)
	{
		var comment = await context.Comments
			.TagWith("Getting comment redirect data")
			.Where(c => c.Id == commentId)
			.Select(c => new CommentMeta
			{
				// Get the ordinal number of the comment within the thread.
				Ordinal = c.CommentThread.Comments.Count(x => x.Id < commentId),
				UserName = c.CommentThread.User != null ? c.CommentThread.User.UserName : null,
				StoryId = c.CommentThread.Chapter != null ? c.CommentThread.Chapter.StoryId : null,
				Slug = c.CommentThread.Chapter != null ? c.CommentThread.Chapter.Slug :
					c.CommentThread.Blogpost != null ? c.CommentThread.Blogpost.Slug : null,
				ClubId = c.CommentThread.ClubThread != null ? c.CommentThread.ClubThread.ClubId : null,
				Id = c.CommentThread.BlogpostId ?? c.CommentThread.ChapterId ?? c.CommentThread.ClubThreadId ?? c.CommentThread.UserId,
				Which = c.CommentThread.UserId != null ? ThreadType.User :
					c.CommentThread.BlogpostId != null ? ThreadType.Blogpost :
					c.CommentThread.ChapterId != null ? ThreadType.Chapter :
					c.CommentThread.ClubThreadId != null ? ThreadType.Club : null,
			})
			.FirstOrDefaultAsync();

		if (comment is null) return null;

		// +1 because they're 1-indexed.
		var frag = new FragmentString($"#comment-{comment.Ordinal + 1}");

		return comment.Which switch
		{
			ThreadType.User when comment.UserName is not null
				=> User_Index.Get(comment.UserName).Path(linkGenerator, fragment: frag),
			ThreadType.Blogpost when comment is { Id: {} cid, Slug: {} sl }
				=> Blog_Post.Get(cid, sl).Path(linkGenerator, fragment: frag),
			ThreadType.Chapter when comment is { StoryId: {} sid, Id: {} cid, Slug: {} sl }
				=> Chapter.Get(sid, cid, sl).Path(linkGenerator, fragment: frag),
			ThreadType.Club when comment is { ClubId: {} clid, Id: {} cid }
				=> Club_Forums_Details.Get(cid, clid).Path(linkGenerator, fragment: frag),
			_ => null,
		};
	}

	private enum ThreadType
	{
		User,
		Blogpost,
		Chapter,
		Club,
	}

	private sealed class CommentMeta
	{
		public required int Ordinal { get; init; }
		public required ThreadType? Which { get; init; }
		public required string? UserName { get; init; }
		public required long? StoryId { get; init; }
		public required string? Slug { get; init; }
		public required long? Id { get; init; }
		public required long? ClubId { get; init; }
	}
}