using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Faqs.Commands;
using Ogma3.Api.V1.Faqs.Queries;
using Ogma3.Data.Faqs;

namespace Ogma3.Api.V1.Faqs;

[Route("api/[controller]", Name = nameof(FaqsController))]
[ApiController]
public class FaqsController : ControllerBase
{
    private readonly IMediator _mediator;
    public FaqsController(IMediator mediator) => _mediator = mediator;
        
    // GET: api/Faqs
    [HttpGet]
    public async Task<ActionResult<List<Faq>>> GetFaqs()
        => await _mediator.Send(new GetAllFaqs.Query());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Faq>> GetFaq(long id)
        => await _mediator.Send(new GetSingleFaq.Query(id));

    // PUT: api/Faqs/5
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutFaq(UpdateFaq.Command data)
        => await _mediator.Send(data);
        
    // POST: api/Faqs
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Faq>> PostFaq(CreateFaq.Command data)
        => await _mediator.Send(data);
        
    // DELETE: api/Faqs/5
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<long>> DeleteFaq(long id)
        => await _mediator.Send(new DeleteFaq.Command(id));
}