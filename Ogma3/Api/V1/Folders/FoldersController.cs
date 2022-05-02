using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Ogma3.Api.V1.Folders.Commands;
using Ogma3.Api.V1.Folders.Queries;
using Ogma3.Data.Folders;

namespace Ogma3.Api.V1.Folders;

[Route("api/[controller]", Name = nameof(FoldersController))]
[ApiController]
public class FoldersController : ControllerBase
{
	private readonly IMediator _mediator;
	public FoldersController(IMediator mediator) => _mediator = mediator;

	// GET api/folders/5
	[HttpGet("{id:long}")]
	[Authorize]
	public async Task<ActionResult<List<GetFolder.Result>>> GetFoldersOfClub(long id)
		=> await _mediator.Send(new GetFolder.Query(id));

	[HttpPost("add-story")]
	[Authorize]
	public async Task<ActionResult<FolderStory>> AddStory(AddStoryToFolder.Command data)
		=> await _mediator.Send(data);

	// Don't delete or this whole controller will break
	[HttpGet, OpenApiIgnore]
	public string Ping() => "Pong";
}