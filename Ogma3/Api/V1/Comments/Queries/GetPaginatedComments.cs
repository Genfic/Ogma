using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Markdig;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments.Queries;

public static class GetPaginatedComments
{
    public sealed record Query(long Thread, int? Page, long? Highlight) : IRequest<ActionResult<PaginationResult<CommentDto>>>;

    public class Handler : IRequestHandler<Query, ActionResult<PaginationResult<CommentDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly OgmaConfig _ogmaConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
            
        public Handler(ApplicationDbContext context, IUserService userService, OgmaConfig ogmaConfig, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userService = userService;
            _ogmaConfig = ogmaConfig;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ActionResult<PaginationResult<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var (thread, page, highlight) = request;
                
            var total = await _context.CommentThreads
                .Where(ct => ct.Id == thread)
                .Select(ct => ct.CommentsCount)
                .FirstOrDefaultAsync(cancellationToken);

            // If a highlight has been requested, get the page on which the highlighted comment would be.
            // If not, simply return the requested page or the first page if requested page is null.
            // `highlight - 1` offsets the fact, that the requested IDs start from 1, not 0
            var p = highlight is null
                ? Math.Max(1, page ?? 1)
                : (int)Math.Ceiling((double)(total - (highlight - 1)) / _ogmaConfig.CommentsPerPage);

            // Send auth data
            _httpContextAccessor.HttpContext?.Response.Headers.Add("X-Authenticated", (_userService.User?.Identity?.IsAuthenticated ?? false).ToString());

            var comments =  await _context.Comments
                .Where(c => c.CommentsThreadId == thread)
                .OrderByDescending(c => c.DateTime)
                .Select(CommentMappings.ToCommentDto(_userService.User?.GetNumericId()))
                .AsNoTracking()
                .Paginate(p, _ogmaConfig.CommentsPerPage)
                .ToListAsync(cancellationToken);
            
            comments.ForEach(c => c.Body = Markdown.ToHtml(c.Body, MarkdownPipelines.Comment));
            
            return new PaginationResult<CommentDto>
            {
                Elements = comments,
                Total = total,
                Page = p,
                Pages = (int)Math.Ceiling((double)total / _ogmaConfig.CommentsPerPage),
                PerPage = _ogmaConfig.CommentsPerPage
            };
        }
    }
}