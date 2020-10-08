using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Utils.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(CommentsController))]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OgmaConfig _ogmaConfig;
        private readonly CommentsRepository _commentsRepo;
        private readonly IMapper _mapper;

        public CommentsController(ApplicationDbContext context, OgmaConfig ogmaConfig, CommentsRepository commentsRepo, IMapper mapper)
        {
            _context = context;
            _ogmaConfig = ogmaConfig;
            _commentsRepo = commentsRepo;
            _mapper = mapper;
        }

        
        // GET: api/Comments?thread=6[&page=1&highlight=10]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<CommentDto>>> GetComments(
            [FromQuery]long thread, 
            [FromQuery]int? page, 
            [FromQuery]long? highlight
        )
        {
            var total = await _commentsRepo.CountComments(thread);

            // If a highlight has been requested, get the page on which the highlighted comment would be.
            // If not, simply return the requested page or the first page if requested page is null
            var p = highlight.HasValue
                ? (int) Math.Ceiling((double) (total - highlight) / _ogmaConfig.CommentsPerPage)
                : Math.Max(1, page ?? 1);

            return new PaginationResult<CommentDto>
            {
                Elements = await _commentsRepo.GetPaginated(thread, p, _ogmaConfig.CommentsPerPage),
                Total = total,
                Page = p,
                Pages = (int)Math.Ceiling((double)total / _ogmaConfig.CommentsPerPage),
                PerPage = _ogmaConfig.CommentsPerPage
            };
        }

        [HttpGet("revisions/{id}")]
        public async Task<IEnumerable<CommentRevisionDto>> GetRevisions(long id)
        {
            return await _context.CommentRevisions
                .Where(r => r.ParentId == id)
                .ProjectTo<CommentRevisionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<CommentDto> GetComment(long id)
        {
            return await _commentsRepo.GetSingle(id);
        }

        // GET: api/Comments/md?id=5
        [HttpGet("md")]
        public async Task<string> GetMarkdown([FromQuery]long id)
        {
            return await _commentsRepo.GetMarkdown(id);
        }

        // PATCH: api/Comments
        [HttpPatch]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PutComment(PatchData data)
        {
            var uid = User.GetNumericId();
            var comm = _context.Comments.FirstOrDefault(c => c.Id == data.Id);
            
            if (comm == null) return NotFound();
            if (uid != comm.AuthorId) return Unauthorized();
            
            // Create revision
            await _context.CommentRevisions.AddAsync(new CommentRevision
            {
                Body = comm.Body,
                ParentId = comm.Id,
                EditTime = DateTime.Now
            });

            // Edit the comment
            comm.Id = data.Id;
            comm.Body = data.Body;
            comm.LastEdit = DateTime.Now;
            comm.EditCount = (ushort?)(comm.EditCount + 1) ?? 1;
            
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        // POST: api/Comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<CommentDto>> PostComment(PostData data)
        {
            var uid = User.GetNumericId();

            if (uid == null) return Unauthorized();
            
            var comment = new Comment
            {
                AuthorId = (long) uid,
                Body = data.Body
            };

            var thread = await _context.CommentThreads
                .Where(ct => ct.Id == data.Thread)
                .Include(ct => ct.Comments)
                .FirstOrDefaultAsync();

            if (thread == null) return NotFound();
            
            thread.Comments.Add(comment);
            thread.CommentsCount = thread.Comments.Count;

            await _context.SaveChangesAsync();

            var dto = _mapper.Map<Comment, CommentDto>(comment);
            return CreatedAtAction("GetComment", new { id = comment.Id }, dto);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<CommentDto>> DeleteComment(long id)
        {
            var uid = User.GetNumericId();
            
            var comment = await _context.Comments
                .Where(c => c.Id == id)
                .Include(c => c.Revisions)
                .FirstOrDefaultAsync();
            
            if (comment == null) return NotFound();
            if (comment.AuthorId != uid) return Unauthorized();

            // Wipe comment
            comment.Author = null;
            comment.AuthorId = null;
            comment.DeletedBy = EDeletedBy.User;
            comment.DeletedByUserId = uid;
            comment.Body = string.Empty;
            comment.LastEdit = null;
            comment.EditCount = null;
            
            // Wipe revisions
            comment.Revisions.Clear();

            await _context.SaveChangesAsync();

            return _mapper.Map<Comment, CommentDto>(comment);
        }

        
        // Data classes
        public class PostData
        {
            public string Body { get; set; }
            public long Thread { get; set; }
        }

        public class PatchData
        {
            public long Id { get; set; }
            public string Body { get; set; }
        }
    }
}
