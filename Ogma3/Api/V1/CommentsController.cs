using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
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

        public class GetCommentsInput
        {
            public long Thread { get; set; }
            private int? _page;
            public int Page
            {
                get => Math.Max(1, _page ?? 1);
                set => _page = value;
            }
        }
        public class PaginationResults
        {
            public IEnumerable<CommentDto> Comments { get; set; }
            public int TotalComments { get; set; }
        }

        // GET: api/Comments?thread=6[&page=1&per-page=10]
        [HttpGet]
        public async Task<PaginationResults> GetComments([FromQuery]GetCommentsInput input)
        {
            return new PaginationResults
            {
                Comments = await _commentsRepo.GetPaginated(input.Thread, input.Page, _ogmaConfig.CommentsPerPage),
                TotalComments = await _commentsRepo.CountComments(input.Thread)
            };
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<CommentDto> GetComment(long id)
        {
            return await _commentsRepo.GetSingle(id);
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PutComment(long id, Comment comment, string body)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            var comm = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comm != null) comm.Body = body;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // POST: api/Comments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<CommentDto>> PostComment(CommentFromApiDTO data)
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
        [Authorize]
        public async Task<ActionResult<CommentDto>> DeleteComment(long id)
        {
            var uid = User.GetNumericId();
            
            var comment = await _context.Comments.FindAsync(id);
            
            if (comment == null) return NotFound();
            if (comment.AuthorId != uid) return Unauthorized();

            var thread = await _context.CommentThreads
                .Where(ct => ct.Id == comment.CommentsThreadId)
                .Include(ct => ct.Comments)
                .FirstOrDefaultAsync();
            
            if (thread == null) return NotFound();
            
            _context.Comments.Remove(comment);
            thread.CommentsCount = thread.Comments.Count;
            
            await _context.SaveChangesAsync();

            return _mapper.Map<Comment, CommentDto>(comment);
        }
    }
}
