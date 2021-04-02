using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(FaqsController))]
    [ApiController]
    public class FaqsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FaqsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Faqs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Faq>>> GetFaqs()
        {
            return await _context.Faqs
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Faq>> GetFaq(long id)
        {
            return await _context.Faqs.FindAsync(id);
        }

        // PUT: api/Faqs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutFaq(long id, Faq f)
        {
            if (id != f.Id)
            {
                return BadRequest();
            }

            _context.Entry(new Faq
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer,
                AnswerRendered = Markdown.ToHtml(f.Answer, MarkdownPipelines.All)
            }).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaqExists(id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
            {
                return Conflict(new {message = $"A FAQ with the ID '{f.Id}' already exists"});
            }

            return NoContent();
        }


        // POST: api/Faqs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Faq>> PostFaq(Faq f)
        {
            await _context.Faqs.AddAsync(new Faq
            {
                Question = f.Question,
                Answer = f.Answer,
                AnswerRendered = Markdown.ToHtml(f.Answer, MarkdownPipelines.All)
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
            {
                return Conflict(new {message = $"A FAQ with the ID '{f.Id}' already exists"});
            }

            return CreatedAtAction("GetFaq", new {id = f.Id}, f);
        }


        // DELETE: api/Faqs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Faq>> DeleteFaq(long id)
        {
            var f = await _context.Faqs.FindAsync(id);
            if (f == null)
            {
                return NotFound();
            }

            _context.Faqs.Remove(f);
            await _context.SaveChangesAsync();

            return f;
        }

        private bool FaqExists(long id)
        {
            return _context.Faqs.Any(e => e.Id == id);
        }
    }
}