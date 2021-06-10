using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(QuotesController))]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quote>> GetQuotes()
        {
            return await _context.Quotes.ToListAsync();
        }

        // GET: api/Quotes/5
        [HttpGet("{id:int}")]
        [Throttle(Count = 10, TimeUnit = TimeUnit.Minute)]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            var quote = await _context.Quotes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote is null)
            {
                return NotFound();
            }

            return quote;
        }

        
        // GET: api/Quotes/random
        [HttpGet("random")]
        [Throttle(Count = 1, TimeUnit = TimeUnit.Second)]
        public async Task<ActionResult<Quote>> GetRandomQuote()
        {
            var q = (await _context.Quotes
                .FromSqlRaw(@"
                    SELECT *
                    FROM ""Quotes""
                    OFFSET floor(random() * (
                        SELECT count(*)
                        FROM ""Quotes""
                    ))
                    LIMIT 1
                ")
                .AsNoTracking()
                .ToListAsync())
                .First();
            
            if (q is null) return NotFound();
            return q;
        }
        

        // PUT: api/Quotes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Quote>> PutQuote(int id, Quote q)
        {
            if (id != q.Id)
            {
                return BadRequest();
            }

            _context.Entry(q).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return q;
        }


        // POST: api/Quotes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Quote>> PostQuote(Quote q)
        {
            _context.Quotes.Add(q);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetQuote", new { id = q.Id }, q);
        }

        // POST: api/Quotes/json
        [HttpPost("json")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostJson()
        {
            var data = await JsonSerializer.DeserializeAsync<IEnumerable<Quote>>(Request.Body);
            if (data is null) return BadRequest();
            
            await _context.Quotes.AddRangeAsync(data);
            await _context.SaveChangesAsync();
            
            return Ok();
        }


        // DELETE: api/Quotes/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Quote>> DeleteQuote(int id)
        {
            var ns = await _context.Quotes.FindAsync(id);
            if (ns is null) return NotFound();
            
            _context.Quotes.Remove(ns);
            await _context.SaveChangesAsync();

            return ns;
        }

        private bool QuoteExists(int id)
        {
            return _context.Quotes.Any(e => e.Id == id);
        }
    }
}
