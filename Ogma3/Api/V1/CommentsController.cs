using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(CommentsController))]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<OgmaUser> _userManager;
        private readonly OgmaConfig _ogmaConfig;

        public CommentsController(ApplicationDbContext context, UserManager<OgmaUser> userManager, OgmaConfig ogmaConfig)
        {
            _context = context;
            _userManager = userManager;
            _ogmaConfig = ogmaConfig;
        }

        public class GetCommentsInput
        {
            public long Thread { get; set; }
            private int? _page;
            private int? _perPage;
            public int Page
            {
                get => Math.Max(1, _page ?? 1);
                set => _page = value;
            }
            public int PerPage
            {
                get => _perPage?.Clamp(1, 100) ?? 20;
                set => _perPage = value;
            }
        }

        // GET: api/Comments?thread=6[&page=1&per-page=10]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments([FromQuery]GetCommentsInput input)
        {
            var comments =  await _context.Comments
                .Where(c => c.CommentsThreadId == input.Thread)
                .Include(c => c.Author)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                // .Skip((input.Page - 1) * input.PerPage).Take(input.PerPage) // Pagination
                .Select(c => new CommentDTO(c, true))
                .AsNoTracking()
                .ToListAsync();
            
            return comments.Select(c =>
            {
                c.Author.Avatar = _ogmaConfig.Cdn + c.Author.Avatar;
                return c;
            }).ToList();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDTO>> GetComment(long id)
        {
            var comment = await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            var dto = new CommentDTO(comment);
            dto.Author.Avatar = _ogmaConfig.Cdn + dto.Author.Avatar;
            return dto;
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/Comments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<CommentDTO>> PostComment(CommentFromApiDTO data)
        {
            var comment = new Comment
            {
                Author = await _userManager.GetUserAsync(User),
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

            var dto = new CommentDTO(comment);
            dto.Author.Avatar = _ogmaConfig.Cdn + dto.Author.Avatar;
            return CreatedAtAction("GetComment", new { id = comment.Id }, dto);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<CommentDTO>> DeleteComment(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _context.Comments.FindAsync(id);
            
            if (comment == null) return NotFound();
            if (comment.Author != user) return Forbid();

            var thread = await _context.CommentThreads
                .Where(ct => ct.Id == comment.CommentsThreadId)
                .Include(ct => ct.Comments)
                .FirstOrDefaultAsync();
            
            if (thread == null) return NotFound();
            
            _context.Comments.Remove(comment);
            thread.CommentsCount = thread.Comments.Count;
            
            await _context.SaveChangesAsync();

            var dto = new CommentDTO(comment);
            dto.Author.Avatar = _ogmaConfig.Cdn + dto.Author.Avatar;
            return dto;
        }

        private bool CommentExists(long id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
