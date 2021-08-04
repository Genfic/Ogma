using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Shelves.Commands;
using Ogma3.Api.V1.Shelves.Queries;
using Ogma3.Data.Shelves;

namespace Ogma3.Api.V1.Shelves
{
    [Route("api/[controller]", Name = nameof(ShelvesController))]
    [ApiController]
    public class ShelvesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ShelvesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ShelfDto>> GetShelf(long id)
            => await _mediator.Send(new GetShelf.Query(id));
        
        // GET: api/Shelves/JohnSmith?page=1
        [HttpGet("{name}")]
        public async Task<ActionResult<List<ShelfDto>>> GetUserShelves(string name, [FromQuery] int page)
            => await _mediator.Send(new GetPaginatedUserShelves.Query(name, page));

        // POST: api/Shelves
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ShelfDto>> PostShelf(CreateShelf.Command data)
            => await _mediator.Send(data);
        
        // PUT: api/Shelves/5
        [HttpPut]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<ShelfDto>> PutShelf(UpdateShelf.Command data)
            => await _mediator.Send(data);
        
        // DELETE: api/Shelves/5
        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<ActionResult<long>> DeleteShelf(long id)
            => await _mediator.Send(new DeleteShelf.Command(id));
    }
}