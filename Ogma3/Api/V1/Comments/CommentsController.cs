using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Api.V1.Comments.Commands;
using Ogma3.Api.V1.Comments.Queries;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.ClubThreads;
using Ogma3.Data.Comments;
using Ogma3.Data.Users;
using Ogma3.Infrastructure;

namespace Ogma3.Api.V1.Comments;

[Route("api/[controller]", Name = nameof(CommentsController))]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommentsController(IMediator mediator) => _mediator = mediator;

    // GET
    [HttpGet]
    public async Task<ActionResult<PaginationResult<CommentDto>>> GetCommentsAsync([FromQuery] GetPaginatedComments.Query query)
        => await _mediator.Send(query);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CommentDto>> GetComment(long id)
        => await _mediator.Send(new GetComment.Query(id));

    [HttpGet("revisions/{id:long}")]
    public async Task<ActionResult<IEnumerable<GetRevision.Result>>> GetRevisions(long id)
        => await _mediator.Send(new GetRevision.Query(id));

    [HttpGet("md")]
    public async Task<ActionResult<string>> GetMarkdown([FromQuery] GetCommentMarkdown.Query query)
        => await _mediator.Send(query);

    // POST
    [HttpPost]
    [Authorize, ValidateAntiForgeryToken]
    public async Task<ActionResult<CommentDto>> PostComments(PostData data) => data.Type switch
    {
        nameof(Chapter) => await PostChapterComment(data),
        nameof(OgmaUser) => await PostProfileComment(data),
        nameof(Blogpost) => await PostBlogpostComment(data),
        nameof(ClubThread) => await PostClubThreadComment(data),
        _ => throw new ArgumentOutOfRangeException(nameof(data), $"Was {data.Type}")
    };

    private async Task<ActionResult<CommentDto>> PostChapterComment(PostData data)
        => await _mediator.Send(new CreateChapterComment.Command(data.Body, data.Thread));

    private async Task<ActionResult<CommentDto>> PostProfileComment(PostData data)
        => await _mediator.Send(new CreateProfileComment.Command(data.Body, data.Thread));

    private async Task<ActionResult<CommentDto>> PostBlogpostComment(PostData data)
        => await _mediator.Send(new CreateBlogpostComment.Command(data.Body, data.Thread));

    private async Task<ActionResult<CommentDto>> PostClubThreadComment(PostData data)
        => await _mediator.Send(new CreateClubThreadComment.Command(data.Body, data.Thread));

    public sealed record PostData(string Body, long Thread, string Type);

    // Patch
    [HttpPatch]
    [Authorize, ValidateAntiForgeryToken]
    public async Task<ActionResult<CommentDto>> PutComment(UpdateComment.Command command)
        => await _mediator.Send(command);

    // Delete
    [HttpDelete("{id:long}")]
    [Authorize, ValidateAntiForgeryToken]
    public async Task<ActionResult<long>> DeleteComment(long id)
        => await _mediator.Send(new DeleteComment.Command(id));
}