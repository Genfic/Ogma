using System.Runtime.CompilerServices;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.CommentThreads;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<GetThreadDetails.Result>>;

[Handler]
[MapGet("api/CommentsThread/{threadId:long}")]
public static partial class GetThreadDetails
{
	[Validate]
	[UsedImplicitly]
	public sealed partial record Query(long ThreadId) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		IHttpContextAccessor httpContextAccessor,
		OgmaConfig config,
		CancellationToken cancellationToken
	)
	{
		var isStaff = userService.User?.HasAnyRole(RoleNames.Admin, RoleNames.Moderator) ?? false;

		var perPage = config.CommentsPerPage;
		const int minCommentLength = CTConfig.Comment.MinBodyLength;
		const int maxCommentLength = CTConfig.Comment.MaxBodyLength;

		var threadData = await context.CommentThreads
			.Where(ct => ct.Id == request.ThreadId)
			.Select(ct => new
			{
				Blogpost = ct.BlogpostId != null,
				Profile = ct.UserId != null,
				Chapter = ct.ChapterId != null,
				Thread = ct.ClubThreadId != null,
				Locked = ct.IsLocked,
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (threadData is null) return TypedResults.NotFound();

		var source = threadData switch
		{
			{ Blogpost: true } => CommentSource.Blogpost,
			{ Profile: true } => CommentSource.Profile,
			{ Chapter: true } => CommentSource.Chapter,
			{ Thread: true } => CommentSource.ForumPost,
			_ => throw new SwitchExpressionException(),
		};

		if (isStaff)
		{
			httpContextAccessor.HttpContext?.Response.Headers.Append("X-IsStaff", isStaff.ToString());
		}

		return TypedResults.Ok(new Result(perPage, minCommentLength, maxCommentLength, source, threadData.Locked));
	}

	public sealed record Result(int PerPage, int MinCommentLength, int MaxCommentLength, CommentSource Source, bool IsLocked);
}