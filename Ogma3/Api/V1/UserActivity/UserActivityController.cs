using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.UserActivity.Commands;

namespace Ogma3.Api.V1.UserActivity
{
    [Route("api/[controller]", Name = nameof(UserActivityController))]
    [ApiController]
    public class UserActivityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserActivityController(IMediator mediator) => _mediator = mediator;

        // POST
        [HttpHead]
        public async Task<IActionResult> UpdateLastActiveAsync()
            => await _mediator.Send(new UpdateLastActive.Query());
        
        // Don't delete or this whole controller will break
        [HttpGet] public string Ping() => "Pong";
    }
}