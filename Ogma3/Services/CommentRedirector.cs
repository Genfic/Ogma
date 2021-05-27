using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services
{
    public class CommentRedirector
    {
        private readonly ApplicationDbContext _context;

        public CommentRedirector(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommentRedirectionResult> RedirectToComment(long commentId)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == commentId)
                .Select(c => new CommentMeta
                {
                    // Get the ordinal number of the comment within the thread.
                    // +1 because they're 1-indexed.
                    Ordinal = c.CommentsThread.Comments
                        .OrderBy(x => x.DateTime)
                        .Select(x => x.Id)
                        .ToList()
                        .IndexOf(commentId) + 1,
                    UserName = c.CommentsThread.User.UserName,
                    BlogpostId = c.CommentsThread.BlogpostId,
                    ChapterId = c.CommentsThread.ChapterId,
                    ClubThreadId = c.CommentsThread.ClubThreadId,
                    ClubId = c.CommentsThread.ClubThread.ClubId,
                })
                .FirstOrDefaultAsync();

            if (comment is null) return null;
            
            var order = comment.Ordinal;
            
            // Figure out the redirect
            if (comment.UserName is not null)
                return new CommentRedirectionResult("/User/Index", new { Name = comment.UserName }, $"comment-{order}");
            if (comment.BlogpostId is not null)
                return new CommentRedirectionResult("/Blog/Post", new { Id = comment.BlogpostId }, $"comment-{order}");
            if (comment.ChapterId is not null)
                return new CommentRedirectionResult("/Chapter",new {Id = comment.ChapterId}, $"comment-{order}");
            if (comment.ClubThreadId is not null)
                return new CommentRedirectionResult("/Club/Forums/Details", new { comment.ClubId, ThreadId = comment.ClubThreadId}, $"comment-{order}");

            return null;
        }
        public record CommentRedirectionResult(string Url, object Params, string Fragment);

        private record CommentMeta
        {
            public int Ordinal { get; init; }
            public string UserName { get; init; }
            public long? BlogpostId { get; init; }
            public long? ChapterId { get; init; }
            public long? ClubThreadId { get; init; }
            public long? ClubId { get; init; }
        }
    }
}