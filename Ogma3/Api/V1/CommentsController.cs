using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services;
using Utils.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(CommentsController))]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaConfig _ogmaConfig;
        private readonly NotificationsRepository _notificationsRepo;
        private readonly CommentRedirector _redirector;
        private readonly IMapper _mapper;

        public CommentsController(ApplicationDbContext context, OgmaConfig ogmaConfig, NotificationsRepository notificationsRepo, CommentRedirector redirector, IMapper mapper)
        {
            _context = context;
            _ogmaConfig = ogmaConfig;
            _notificationsRepo = notificationsRepo;
            _redirector = redirector;
            _mapper = mapper;
        }


        // GET: api/Comments?thread=6[&page=1&highlight=10]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<CommentDto>>> GetComments(
            [FromQuery] long thread,
            [FromQuery] int? page,
            [FromQuery] long? highlight
        )
        {
            var total = await _context.CommentThreads
                .Where(ct => ct.Id == thread)
                .Select(ct => ct.CommentsCount)
                .FirstOrDefaultAsync();

            // If a highlight has been requested, get the page on which the highlighted comment would be.
            // If not, simply return the requested page or the first page if requested page is null.
            // `highlight - 1` offsets the fact, that the requested IDs start from 1, not 0
            var p = highlight.HasValue
                ? (int)Math.Ceiling((double)(total - (highlight - 1)) / _ogmaConfig.CommentsPerPage)
                : Math.Max(1, page ?? 1);

            // Send auth data
            Response.Headers.Add("X-Authenticated", (User?.Identity?.IsAuthenticated ?? false).ToString());

            var comments =  await _context.Comments
                .Where(c => c.CommentsThreadId == thread)
                .OrderByDescending(c => c.DateTime)
                .Select(CommentMappings.ToCommentDto(User?.GetNumericId()))
                .AsNoTracking()
                .Paginate(p, _ogmaConfig.CommentsPerPage)
                .ToListAsync();
            
            comments.ForEach(c => c.Body = c.Body is null ? null : Markdown.ToHtml(c.Body, MarkdownPipelines.Comment));
            
            return new PaginationResult<CommentDto>
            {
                Elements = comments,
                Total = total,
                Page = p,
                Pages = (int)Math.Ceiling((double)total / _ogmaConfig.CommentsPerPage),
                PerPage = _ogmaConfig.CommentsPerPage
            };
        }

        [HttpGet("revisions/{id:long}")]
        public async Task<IEnumerable<CommentRevisionDto>> GetRevisions(long id)
        {
            return await _context.CommentRevisions
                .Where(r => r.ParentId == id)
                .ProjectTo<CommentRevisionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id:long}")]
        public async Task<CommentDto> GetComment(long id)
        {
            var comment =  await _context.Comments
                .Where(c => c.Id == id)       
                .Select(CommentMappings.ToCommentDto(User.GetNumericId()))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            comment.Body = comment.Body is null 
                ? null 
                : Markdown.ToHtml(comment.Body, MarkdownPipelines.Comment);
            return comment;
        }

        // GET: api/Comments/md?id=5
        [HttpGet("md")]
        public async Task<string> GetMarkdown([FromQuery] long id)
            => await _context.Comments
                .Where(c => c.Id == id)
                .Select(c => c.Body)
                .FirstOrDefaultAsync();

        // PATCH: api/Comments
        [HttpPatch]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<CommentDto>> PutComment(PatchData data)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            var (body, commentId) = data;

            var comm = _context.Comments.FirstOrDefault(c => c.Id == commentId);

            if (comm is null) return NotFound();
            if (uid != comm.AuthorId) return Unauthorized();

            // Create revision
            _context.CommentRevisions.Add(new CommentRevision
            {
                Body = comm.Body,
                ParentId = comm.Id,
                EditTime = DateTime.Now
            });

            // Edit the comment
            comm.Id = commentId;
            comm.Body = body;
            comm.LastEdit = DateTime.Now;
            comm.EditCount += 1;

            await _context.SaveChangesAsync();

            var dto = _mapper.Map<Comment, CommentDto>(comm);
            dto.Owned = uid == comm.AuthorId;

            return dto;
        }

        public sealed record PatchData(string Body, long Id);

        // POST: api/Comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<CommentDto>> PostComment(PostData data)
        {
            var uid = User?.GetNumericId();
            if (uid is null) return Unauthorized();

            var (body, threadId) = data;

            var comment = new Comment
            {
                AuthorId = (long)uid,
                Body = body
            };

            var thread = await _context.CommentThreads
                .Where(ct => ct.Id == threadId)
                .Include(ct => ct.Comments)
                .FirstOrDefaultAsync();

            if (thread is null) return NotFound();
            if (thread.LockDate is not null) return Unauthorized();

            thread.Comments.Add(comment);
            thread.CommentsCount = thread.Comments.Count;

            await _context.SaveChangesAsync();

            if (thread.UserId != uid)
            {
                // Create notification
                var subscribers = await _context.CommentsThreadSubscribers
                    .Where(cts => cts.CommentsThreadId == thread.Id)
                    .Select(cts => cts.OgmaUserId)
                    .ToListAsync();

                var redirection = await _redirector.RedirectToComment(comment.Id);
                if (redirection is not null)
                {
                    await _notificationsRepo.Create(ENotificationEvent.WatchedThreadNewComment,
                        subscribers,
                        redirection.Url,
                        redirection.Params,
                        redirection.Fragment,
                        comment.Body.Truncate(50)
                    );
                }

                await _context.SaveChangesAsync();
            }

            var dto = _mapper.Map<Comment, CommentDto>(comment);
            return CreatedAtAction("GetComment", new { id = comment.Id }, dto);
        }

        public sealed record PostData(string Body, long Thread);

        // DELETE: api/Comments/5
        [HttpDelete("{id:long}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<CommentDto>> DeleteComment(long id)
        {
            var uid = User?.GetNumericId();
            if (uid is null) return Unauthorized();

            var comment = await _context.Comments
                .Where(c => c.Id == id)
                .Include(c => c.Revisions)
                .FirstOrDefaultAsync();

            if (comment is null) return NotFound();
            if (comment.AuthorId != uid) return Unauthorized();

            // Wipe comment
            comment.Author = null;
            comment.AuthorId = null;
            comment.DeletedBy = EDeletedBy.User;
            comment.DeletedByUserId = uid;
            comment.Body = string.Empty;
            comment.LastEdit = null;
            comment.EditCount = 0;

            // Wipe revisions
            comment.Revisions.Clear();

            await _context.SaveChangesAsync();

            return _mapper.Map<Comment, CommentDto>(comment);
        }
    }
}