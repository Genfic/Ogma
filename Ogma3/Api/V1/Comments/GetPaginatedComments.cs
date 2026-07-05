using System.IO.Hashing;
using System.Text;
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
using Ogma3.Infrastructure.IResults;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Services.UserService;
using Sqids;

namespace Ogma3.Api.V1.Comments;

using ReturnType = Results<Ok<PaginationResult<CommentDto>>, NotModifiedResult, NotFound>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("comments")]
public sealed partial class GetPaginatedComments
(
	ApplicationDbContext context,
	IUserService userService,
	OgmaConfig ogmaConfig,
	IHttpContextAccessor httpContextAccessor,
	SqidsEncoder<long> sqids)
{
	private const string HeaderName = "X-Username";

	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			// .Produces(StatusCodes.Status304NotModified)
			.WithHeader("200", HeaderName, "The username of the user who is requesting the comments or null if the request is anonymous.")
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		var ctx = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");

		var uid = userService.UserId;

		var (thread, page) = request;

		var lastModified = await context.CommentThreads
			.Where(ct => ct.Id == thread)
			.Select(ct => ct.LastChange)
			.FirstOrDefaultAsync(cancellationToken);

		if (lastModified == default)
		{
			return TypedResults.NotFound();
		}

		var etag = GenerateETag(thread, page ?? 1, uid, lastModified);

		ctx.Response.Headers.CacheControl = "private, no-cache";
		ctx.Response.Headers.ETag = etag;

		if (etag == ctx.Request.Headers.IfNoneMatch.ToString())
		{
			return TypedResults.NotModified();
		}

		var total = await context.Comments
			.Where(c => c.CommentsThreadId == thread)
			.CountAsync(cancellationToken);

		// Send auth data
		ctx.Response.Headers.Append(HeaderName, userService.User?.GetUsername());

		var comments = await context.Comments
			.Where(c => c.CommentsThreadId == thread)
			.OrderByDescending(c => c.DateTime)
			.Select(CommentMappings.ToCommentDto(uid))
			.Paginate(page ?? 1, ogmaConfig.CommentsPerPage)
			.ToListAsync(cancellationToken);

		foreach (var comment in comments)
		{
			comment.Id = sqids.Encode(comment.InternalId);

			if (comment.Author is null)
			{
				continue;
			}

			var src = comment.Author.Avatar;
			comment.Author.Avatar = src.AsSpan()[..2] switch
			{
				['/', _] => src,
				['h', 't'] => $"https://genfic.net/cdn-cgi/image/h={120},w={120},format=auto,fit=cover/{src}",
				_ => $"https://genfic.net/cdn-cgi/image/h={120},w={120},format=auto,fit=cover/{Path.Join(ogmaConfig.Cdn, src)}",
			};
		}

		var pagination = new PaginationResult<CommentDto>
		{
			Elements = comments,
			Total = total,
			Page = page ?? 1,
			Pages = (int)Math.Ceiling((double)total / ogmaConfig.CommentsPerPage),
			PerPage = ogmaConfig.CommentsPerPage,
		};

		return TypedResults.Ok(pagination);
	}

	private static string GenerateETag(long threadId, int page, long? userId, DateTimeOffset lastModified)
	{
		var raw = $"{threadId}:{page}:{userId?.ToString() ?? "anon"}:{lastModified.Ticks}";
		var hash = XxHash64.HashToUInt64(Encoding.UTF8.GetBytes(raw));
		return $"\"{hash:x16}\"";
	}

	[Validate]
	[UsedImplicitly]
	public sealed partial record Query(long Thread, int? Page) : IValidationTarget<Query>;
}