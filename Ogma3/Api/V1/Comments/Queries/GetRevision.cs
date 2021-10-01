using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Markdig;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Comments.Queries;

public static class GetRevision
{
    public sealed record Query(long Id) : IRequest<ActionResult<IEnumerable<Result>>>;

    public class Handler : IRequestHandler<Query, ActionResult<IEnumerable<Result>>>
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<ActionResult<IEnumerable<Result>>> Handle(Query request, CancellationToken cancellationToken) 
            => await _context.CommentRevisions
                .Where(r => r.ParentId == request.Id)
                .Select(r => new Result(
                    r.EditTime, 
                    Markdown.ToHtml(r.Body, MarkdownPipelines.Comment, null)    
                ))
                .ToArrayAsync(cancellationToken);
    }

    public sealed record Result(DateTime EditTime, string Body);
}