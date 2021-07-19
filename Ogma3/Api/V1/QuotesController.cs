using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Quotes;
using Ogma3.Data.Quotes.Commands;
using Ogma3.Data.Quotes.Queries;
using Ogma3.Infrastructure.Attributes;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(QuotesController))]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuotesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEnumerable<Quote>> GetQuotes()
        {
            return await _mediator.Send(new GetAll.Query());
        }

        // GET: api/Quotes/5
        [HttpGet("{id:int}")]
        [Throttle(Count = 10, TimeUnit = TimeUnit.Minute)]
        public async Task<IActionResult> GetQuote(int id)
        {
            var quote = await _mediator.Send(new GetOne.Query(id));
            return quote is null ? NotFound() : Ok(quote);
        }


        // GET: api/Quotes/random
        [HttpGet("random")]
        [Throttle(Count = 1, TimeUnit = TimeUnit.Second)]
        public async Task<IActionResult> GetRandomQuote()
        {
            var quote = await _mediator.Send(new GetRandom.Query());
            return quote is null ? NotFound() : Ok(quote);
        }

        
        // POST: api/Quotes
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostQuote(Create.Command q)
        {
            var quote = await _mediator.Send(q);
            return CreatedAtAction("GetQuote", new { quote.Id }, quote);
        }

        // PUT: api/Quotes/5
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutQuote(Update.Command q)
        {
            var quote = await _mediator.Send(q);
            return quote is null ? BadRequest() : Ok(quote);
        }

        // POST: api/Quotes/json
        [HttpPost("json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostJson()
        {
            var res = await _mediator.Send(new CreateFromJson.Command(Request.Body));
            return res ? Ok() : BadRequest();
        }


        // DELETE: api/Quotes/5
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<Quote>> DeleteQuote(Delete.Command q)
        {
            var quote = await _mediator.Send(q);
            return quote is null ? StatusCode(500) : Ok(quote);
        }
    }
}