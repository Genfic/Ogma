using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.ContentBlocks.Commands;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.ContentBlocks
{
    [Route("api/[controller]", Name = nameof(ContentBlockController))]
    [ApiController]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class ContentBlockController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ContentBlockController(IMediator mediator) => _mediator = mediator;

        [HttpPost("story")]
        public async Task<ActionResult> BlockStory(BlockContent.Command<Story> data) 
            => await _mediator.Send(data);
        
        [HttpPost("chapter")]
        public async Task<ActionResult> BlockChapter(BlockContent.Command<Chapter> data)
            => await _mediator.Send(data);
        
        [HttpPost("blogpost")]
        public async Task<ActionResult> BlockBlogpost(BlockContent.Command<Blogpost> data)
            => await _mediator.Send(data);

        // Don't delete or this whole controller will break
        [HttpGet] public string Ping() => "Pong";
    }
}