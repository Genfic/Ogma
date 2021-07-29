using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.SignIn.Queries;

namespace Ogma3.Api.V1.SignIn
{
    [Route("api/[controller]", Name = nameof(SignInController))]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SignInController(IMediator mediator) => _mediator = mediator;

        // GET
        [HttpGet]
        public async Task<ActionResult<GetSignInData.Result>> GetSignInAsync(string name)
            => await _mediator.Send(new GetSignInData.Query(name));
    }
}