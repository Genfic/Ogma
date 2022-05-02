﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Quotes.Commands;
using Ogma3.Api.V1.Quotes.Queries;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Quotes;

[Route("api/[controller]", Name = nameof(QuotesController))]
[ApiController]
public class QuotesController : ControllerBase
{
	private readonly IMediator _mediator;
	public QuotesController(IMediator mediator) => _mediator = mediator;

	[HttpGet]
	public async Task<ActionResult<List<Quote>>> GetQuotes()
		=> await _mediator.Send(new GetAll.Query());

	// GET: api/Quotes/5
	[HttpGet("{id:long}")]
	[Throttle(Count = 10, TimeUnit = TimeUnit.Minute)]
	public async Task<ActionResult<QuoteDto>> GetQuote(long id)
		=> await _mediator.Send(new GetOne.Query(id));

	// GET: api/Quotes/random
	[HttpGet("random")]
	[Throttle(Count = 1, TimeUnit = TimeUnit.Second)]
	public async Task<ActionResult<QuoteDto>> GetRandomQuote()
		=> await _mediator.Send(new GetRandom.Query());


	// POST: api/Quotes
	[HttpPost]
	[Authorize(Roles = RoleNames.Admin)]
	[IgnoreAntiforgeryToken]
	public async Task<ActionResult<Quote>> PostQuote(CreateQuote.Command q)
		=> await _mediator.Send(q);

	// PUT: api/Quotes/5
	[HttpPut]
	[Authorize(Roles = RoleNames.Admin)]
	[IgnoreAntiforgeryToken]
	public async Task<ActionResult<Quote>> PutQuote(UpdateQuote.Command q)
		=> await _mediator.Send(q);

	// POST: api/Quotes/json
	[HttpPost("json")]
	[Authorize(Roles = RoleNames.Admin)]
	[IgnoreAntiforgeryToken]
	[ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<CreateQuotesFromJson.Response>> PostJson()
		=> await _mediator.Send(new CreateQuotesFromJson.Command(Request.Body));

	// DELETE: api/Quotes/5
	[HttpDelete]
	[Authorize(Roles = RoleNames.Admin)]
	[IgnoreAntiforgeryToken]
	public async Task<ActionResult<Quote>> DeleteQuote(DeleteQuote.Command q)
		=> await _mediator.Send(q);
}