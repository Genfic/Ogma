using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.ChaptersReads.Commands;
using Ogma3.Api.V1.ChaptersReads.Queries;

namespace Ogma3.Api.V1.ChaptersReads
{
    [Route("api/[controller]", Name = nameof(ChaptersReadController))]
    [ApiController]
    [Authorize]
    public class ChaptersReadController : Controller
    {
        private readonly IMediator _mediator;
        
        public ChaptersReadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/chaptersread/5
        [HttpGet("{story:long}")]
        public async Task<ActionResult<ICollection<long>>> GetChaptersRead(long story)
            => await _mediator.Send(new GetReadChapters.Query(story));
        
        // POST api/chaptersread
        [HttpPost]
        public async Task<IActionResult> PostChaptersRead(MarkChapterAsRead.Query post)
            => await _mediator.Send(post);

        [HttpDelete]
        public async Task<IActionResult> DeleteChaptersRead(MarkChapterAsUnread.Query post)
            => await _mediator.Send(post);
    }
}