using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Faqs.Commands;
using Ogma3.Api.V1.Faqs.Queries;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Faqs;

[Route("api/[controller]", Name = nameof(FaqsController))]
[ApiController]
public class FaqsController(IMediator mediator) : ControllerBase
{
	// GET: api/Faqs
	[HttpGet]
	public async Task<ActionResult<List<FaqDto>>> GetFaqs()
		=> await mediator.Send(new GetAllFaqs.Query());

	[HttpGet("{id:long}")]
	public async Task<ActionResult<FaqDto>> GetFaq(long id)
		=> await mediator.Send(new GetSingleFaq.Query(id));

	// PUT: api/Faqs/5
	[HttpPut]
	[Authorize(Roles = RoleNames.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> PutFaq(UpdateFaq.Command data)
		=> await mediator.Send(data);

	// POST: api/Faqs
	[HttpPost]
	[Authorize(Roles = RoleNames.Admin)]
	public async Task<ActionResult<FaqDto>> PostFaq(CreateFaq.Command data)
		=> await mediator.Send(data);

	// DELETE: api/Faqs/5
	[HttpDelete("{id:long}")]
	[Authorize(Roles = RoleNames.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<long>> DeleteFaq(long id)
		=> await mediator.Send(new DeleteFaq.Command(id));
}