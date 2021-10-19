using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Tags.Commands;
using Ogma3.Api.V1.Tags.Queries;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Tags;

[Route("api/[controller]", Name = nameof(TagsController))]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TagsController(IMediator mediator) => _mediator = mediator;

    // GET: api/Tags/all
    [HttpGet("all")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Moderator}")]
    public async Task<ActionResult<List<TagDto>>> GetAll()
        => await _mediator.Send(new GetAllTags.Query());

    [HttpGet("search")]
    public ActionResult<List<TagDto>> Search(string name)
    {
        return NotFound(name);
    }

    // GET: api/Tags?page=1&perPage=10
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags([FromQuery] GetPaginatedTags.Query query)
        => await _mediator.Send(query);
        
    // GET: api/Tags/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<TagDto>> GetTag(long id)
        => await _mediator.Send(new GetSingleTag.Query(id));

    // GET: api/Tags/story/5
    [HttpGet("story/{id:long}")]
    public async Task<ActionResult<List<TagDto>>> GetStoryTags(long id)
        => await _mediator.Send(new GetStoryTags.Query(id));
        
    // PUT: api/Tags/5
    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status409Conflict)] 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutTag(UpdateTag.Command data)
        => await _mediator.Send(data);
        
    // POST: api/Tags
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status409Conflict), ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> PostTag(CreateTag.Command data)
        => await _mediator.Send(data);
        
    // DELETE: api/Tags/5
    [HttpDelete("{id:long}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<long>> DeleteTag(long id)
        => await _mediator.Send(new DeleteTag.Command(id));

}