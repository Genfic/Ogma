using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Ratings.Commands;
using Ogma3.Api.V1.Ratings.Queries;
using Ogma3.Data.Ratings;

namespace Ogma3.Api.V1.Ratings
{
    [Route("api/[controller]", Name = nameof(RatingsController))]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RatingsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<RatingApiDto>>> GetRatings()
            => await _mediator.Send(new GetAllRatings.Query());

        [HttpGet("{id:long}")]
        public async Task<ActionResult<RatingApiDto>> GetRating(long id)
            => await _mediator.Send(new GetRatingById.Query(id));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RatingApiDto>> PostRating([FromForm] CreateRating.Command data)
            => await _mediator.Send(data);

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RatingApiDto>> PutRating([FromForm] UpdateRating.Command data)
            => await _mediator.Send(data);

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> DeleteRating(long id)
            => await _mediator.Send(new DeleteRating.Query(id));
    }
}
