using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services;

public class CommentRedirector(ApplicationDbContext context)
{
	public async Task<CommentRedirectionResult?> RedirectToComment(long commentId)
	{
		var comment = await context.Comments
			.Where(c => c.Id == commentId)
			.Select(c => new CommentMeta
			{
				// Get the ordinal number of the comment within the thread.
				Ordinal = c.CommentsThread.Comments
					.OrderBy(x => x.DateTime)
					.Count(x => x.Id < commentId),
				Name = c.CommentsThread.User != null ? c.CommentsThread.User.UserName : null,
				ClubId = c.CommentsThread.ClubThread != null ? c.CommentsThread.ClubThread.ClubId : null,
				Id = c.CommentsThread.BlogpostId ?? c.CommentsThread.ChapterId ?? c.CommentsThread.ClubThreadId,
				Which = c.CommentsThread.UserId != null ? ThreadType.User :
					c.CommentsThread.BlogpostId != null ? ThreadType.Blogpost :
					c.CommentsThread.ChapterId != null ? ThreadType.Chapter :
					c.CommentsThread.ClubThreadId != null ? ThreadType.Club : null,
			})
			.FirstOrDefaultAsync();

		if (comment is null) return null;

		// +1 because they're 1-indexed.
		var order = comment.Ordinal + 1;

		return comment.Which switch
		{
			ThreadType.User => new CommentRedirectionResult("/User/Index", new { comment.Name }, $"comment-{order}"),
			ThreadType.Blogpost => new CommentRedirectionResult("/Blog/Post", new { comment.Id }, $"comment-{order}"),
			ThreadType.Chapter => new CommentRedirectionResult("/Chapter", new { comment.Id }, $"comment-{order}"),
			ThreadType.Club => new CommentRedirectionResult("/Club/Forums/Details", new { comment.ClubId, ThreadId = comment.Id },
				$"comment-{order}"),
			_ => null,
		};
	}

	public record CommentRedirectionResult(string Url, object Params, string Fragment);

	private enum ThreadType
	{
		User,
		Blogpost,
		Chapter,
		Club,
	}

	private record CommentMeta
	{
		public int Ordinal { get; init; }
		public ThreadType? Which { get; init; }
		public string? Name { get; init; }
		public long? Id { get; init; }
		public long? ClubId { get; init; }
	}
}