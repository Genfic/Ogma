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

namespace Ogma3.Api
{
    [Route("api/[controller]", Name = "CommentsController")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Comments?thread=6
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments([FromQuery] int thread)
        {
            return await _context.Comments
                .Where(c => c.CommentsThreadId == thread)
                .Include(c => c.Author)
                .Select(c => CommentDTO.FromComment(c))
                .ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDTO>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return CommentDTO.FromComment(comment);
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutComment(int id, Comment comment, string body)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            var comm = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comm != null) comm.Body = body;

            // _context.Entry(comment).State = EntityState.Modified;

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
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Comments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDTO>> PostComment(CommentFromApiDTO data)
        {
            var comment = new Comment
            {
                Author = await _userManager.GetUserAsync(User),
                Body = data.Body
            };

            var thread = await _context.CommentThreads.FindAsync(data.Thread);
            thread.Comments.Add(comment);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, CommentDTO.FromComment(comment));
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<CommentDTO>> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return CommentDTO.FromComment(comment);
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}