using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services;

public sealed class CommentRedirector(ApplicationDbContext context)
{
	public async Task<CommentRedirectionResult?> RedirectToComment(long commentId)
	{
		var comment = await context.Comments
			.Where(c => c.Id == commentId)
			.Select(c => new CommentMeta
			{
				// Get the ordinal number of the comment within the thread.
				Ordinal = c.CommentThread.Comments
					.OrderBy(x => x.DateTime)
					.Count(x => x.Id < commentId),
				Name = c.CommentThread.User != null ? c.CommentThread.User.UserName : null,
				ClubId = c.CommentThread.ClubThread != null ? c.CommentThread.ClubThread.ClubId : null,
				Id = c.CommentThread.BlogpostId ?? c.CommentThread.ChapterId ?? c.CommentThread.ClubThreadId,
				Which = c.CommentThread.UserId != null ? ThreadType.User :
					c.CommentThread.BlogpostId != null ? ThreadType.Blogpost :
					c.CommentThread.ChapterId != null ? ThreadType.Chapter :
					c.CommentThread.ClubThreadId != null ? ThreadType.Club : null,
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

	private sealed class CommentMeta
	{
		public required int Ordinal { get; init; }
		public required ThreadType? Which { get; init; }
		public required string? Name { get; init; }
		public required long? Id { get; init; }
		public required long? ClubId { get; init; }
	}
}