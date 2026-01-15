using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.ETagService;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<Ok<PaginationResult<CommentDto>>, StatusCodeHttpResult>;

[Handler]
[MapGet("api/comments")]
public static partial class GetPaginatedComments
{
	private const string HeaderName = "X-Username";

	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.WithHeader("200", HeaderName, "The username of the user who is requesting the comments or null if the request is anonymous.");

	[Validate]
	[UsedImplicitly]
	public sealed partial record Query(long Thread, int? Page, long? Highlight) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		OgmaConfig ogmaConfig,
		IHttpContextAccessor httpContextAccessor,
		ETagService eTagService,
		CancellationToken cancellationToken
	)
	{
		var ctx = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");

		var uid = userService.User?.GetNumericId();

		var (thread, page, highlight) = request;
		var etag = eTagService.Get(ETagFor.Comments, request.Thread, uid);

		ctx.Response.Headers.CacheControl = "public, max-age=0, must-revalidate";
		ctx.Response.Headers.ETag = etag.ToString();

		if (ctx.Request.Headers.IfNoneMatch is var inm)
		{
			if (MakeEtag(etag, page, highlight) == inm.ToString())
			{
				return TypedResults.StatusCode(StatusCodes.Status304NotModified);
			}
		}

		var total = await context.Comments
			.Where(c => c.CommentsThreadId == thread)
			.CountAsync(cancellationToken);

		// If a highlight has been requested, get the page on which the highlighted comment would be.
		// If not, return the requested page or the first page if the requested page is null.
		// `highlight - 1` offsets the fact that the requested IDs start from 1, not 0
		var p = highlight is not ({ } h and > 0)
			? Math.Max(1, page ?? 1)
			: (int)Math.Ceiling((total - (h - 1d)) / ogmaConfig.CommentsPerPage);

		// Send auth data
		ctx.Response.Headers.Append(HeaderName, userService.User?.GetUsername());

		var comments = await context.Comments
			.Where(c => c.CommentsThreadId == thread)
			.OrderByDescending(c => c.DateTime)
			.Select(CommentMappings.ToCommentDto(uid))
			.Paginate(p, ogmaConfig.CommentsPerPage)
			.ToListAsync(cancellationToken);

		ctx.Response.Headers.ETag = MakeEtag(etag, page, highlight);

		var pagination = new PaginationResult<CommentDto>
		{
			Elements = comments,
			Total = total,
			Page = p,
			Pages = (int)Math.Ceiling((double)total / ogmaConfig.CommentsPerPage),
			PerPage = ogmaConfig.CommentsPerPage,
		};

		return TypedResults.Ok(pagination);
	}

	private static string MakeEtag(Guid etag, int? page, long? highlight)
	{
		var p = page is not null ? page.ToString() : "n";
		var h = highlight is not null ? highlight.ToString() : "n";
		return $"{etag}-{p}-{h}";
	}
}