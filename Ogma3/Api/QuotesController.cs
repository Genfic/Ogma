using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Models;

namespace Ogma3.Api
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

        // GET: api/Quotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            var q = await _context.Quotes.FindAsync(id);

            if (q == null)
            {
                return NotFound();
            }

            return q;
        }

        
        // GET: api/Quotes/random
        [HttpGet("random")]
        public async Task<ActionResult<Quote>> GetRandomQuote()
        {
            var q = await _context.Quotes
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync();
            if (q == null) return NotFound();
            return q;
        }
        

        // PUT: api/Quotes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
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
            await _context.Quotes.AddRangeAsync(data);
            await _context.SaveChangesAsync();
            return Ok();
        }


        // DELETE: api/Quotes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Quote>> DeleteQuote(int id)
        {
            var ns = await _context.Quotes.FindAsync(id);
            if (ns == null) return NotFound();
            
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
