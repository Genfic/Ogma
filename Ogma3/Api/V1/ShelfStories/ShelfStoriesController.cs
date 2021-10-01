using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.ShelfStories.Commands;
using Ogma3.Api.V1.ShelfStories.Queries;
using Ogma3.Data.Shelves;

namespace Ogma3.Api.V1.ShelfStories;

[Route("api/[controller]", Name = nameof(ShelfStoriesController))]
[ApiController]
public class ShelfStoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ShelfStoriesController(IMediator mediator) => _mediator = mediator;

    // GET: api/ShelfStories/5/quick
    [HttpGet("{storyId:long}/quick")]
    public async Task<ActionResult<List<ShelfDto>>> GetUserQuickShelves(long storyId)
        => await _mediator.Send(new GetCurrentUserQuickShelves.Query(storyId));

    // GET: api/ShelfStories/5
    [HttpGet("{storyId:long}")]
    public async Task<ActionResult<List<GetPaginatedUserShelves.Result>>> GetUserShelvesPaginated(long storyId, [FromQuery] int page)
        => await _mediator.Send(new GetPaginatedUserShelves.Query(storyId, page));
        
    // POST: api/Shelves/5/6
    [HttpPost("{shelfId:long}/{storyId:long}")]
    [Authorize]
    public async Task<ActionResult<AddBookToShelf.Result>> AddToShelf(long shelfId, long storyId)
        => await _mediator.Send(new AddBookToShelf.Command(shelfId, storyId));
        
    // DELETE: api/Shelves/5/6
    [HttpDelete("{shelfId:long}/{storyId:long}")]
    [Authorize]
    public async Task<ActionResult<RemoveBookFromShelf.Result>> RemoveFromShelf(long shelfId, long storyId)
        => await _mediator.Send(new RemoveBookFromShelf.Command(shelfId, storyId));

    [HttpGet, OpenApiIgnore] public string Ping() => "pong";
}