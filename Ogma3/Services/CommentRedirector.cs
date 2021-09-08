using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Services
{
    public class CommentRedirector
    {
        private readonly ApplicationDbContext _context;

        public CommentRedirector(ApplicationDbContext context) => _context = context;

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
                    Name = c.CommentsThread.User.UserName,
                    ClubId = c.CommentsThread.ClubThread.ClubId,
                    Id = c.CommentsThread.BlogpostId ?? c.CommentsThread.ChapterId ?? c.CommentsThread.ClubThreadId,
                    Which = c.CommentsThread.UserId != null ? ThreadType.User :
                        c.CommentsThread.BlogpostId != null ? ThreadType.Blogpost :
                        c.CommentsThread.ChapterId != null ? ThreadType.Chapter :
                        c.CommentsThread.ClubThreadId != null ? ThreadType.Club : null
                })
                .FirstOrDefaultAsync();

            if (comment is null) return null;
            
            var order = comment.Ordinal;

            return comment.Which switch
            {
                ThreadType.User => new CommentRedirectionResult("/User/Index", new { comment.Name }, $"comment-{order}"),
                ThreadType.Blogpost => new CommentRedirectionResult("/Blog/Post", new { comment.Id }, $"comment-{order}"),
                ThreadType.Chapter => new CommentRedirectionResult("/Chapter",new { comment.Id}, $"comment-{order}"),
                ThreadType.Club => new CommentRedirectionResult("/Club/Forums/Details", new { comment.ClubId, ThreadId = comment.Id}, $"comment-{order}"),
                _ => null
            };
        }
        public record CommentRedirectionResult(string Url, object Params, string Fragment);
        
        private enum ThreadType
        {
            User, 
            Blogpost, 
            Chapter, 
            Club
        }
        
        private record CommentMeta
        {
            public int Ordinal { get; init; }
            public ThreadType? Which { get; set; }
            public string? Name { get; set; }
            public long? Id { get; set; }
            public long ClubId { get; set; }
        }
    }
}