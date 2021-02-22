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

        public async Task<CommentRedirectionResult?> RedirectToComment(long commentId)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == commentId)
                .Select(c => new
                {
                    CommentIds = c.CommentsThread.Comments.Select(e => e.Id),
                    c.CommentsThread.User.UserName,
                    c.CommentsThread.BlogpostId,
                    c.CommentsThread.ChapterId,
                    c.CommentsThread.ClubThreadId,
                    c.CommentsThread.ClubThread
                })
                .FirstOrDefaultAsync();

            if (comment is null) return null;

            // Get the ordinal number of the comment within the thread.
            // +1 because they're 1-indexed.
            var order = comment.CommentIds.ToList().IndexOf(commentId) + 1;
            
            // Figure out the redirect
            if (comment.UserName is not null)
                return new CommentRedirectionResult("/User/Index", new { Name = comment.UserName }, $"comment-{order}");
            if (comment.BlogpostId is not null)
                return new CommentRedirectionResult("/Blog/Post", new { Id = comment.BlogpostId }, $"comment-{order}");
            if (comment.ChapterId is not null)
                return new CommentRedirectionResult("/Chapter",new {Id = comment.ChapterId}, $"comment-{order}");
            if (comment.ClubThreadId is not null)
                return new CommentRedirectionResult("/Club/Forums/Details", new { comment.ClubThread.ClubId, ThreadId = comment.ClubThreadId}, $"comment-{order}");

            return null;
        }

        public record CommentRedirectionResult(string Url, object Params, string Fragment);
    }
}