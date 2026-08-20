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
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.CommentThreads;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<GetThreadDetails.Result>>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("CommentsThread/{threadId:long}")]
public sealed partial class GetThreadDetails
	(AppDbContext context, IUserService userService, IHttpContextAccessor httpContextAccessor, OgmaConfig config)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		var isStaff = userService.User?.HasAnyRole(RoleNames.Admin, RoleNames.Moderator) ?? false;

		var perPage = config.CommentsPerPage;

		var threadData = await context.CommentThreads
			.Where(ct => ct.Id == request.ThreadId)
			.Select(ct => new
			{
				Source = ct.Source,
				Locked = ct.IsLocked,
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (threadData is null)
		{
			return TypedResults.NotFound();
		}

		if (isStaff)
		{
			httpContextAccessor.HttpContext?.Response.Headers.Append("X-IsStaff", isStaff.ToString());
		}

		return TypedResults.Ok(new Result(perPage, threadData.Source, threadData.Locked));
	}

	[Validate]
	[UsedImplicitly]
	public sealed partial record Query(long ThreadId) : IValidationTarget<Query>;

	public sealed record Result(int PerPage, CommentSource Source, bool IsLocked);
}