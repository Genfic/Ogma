using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Utils;

namespace Ogma3.Api.V1.SignIn;

[Handler]
[MapGet("api/signin")]
public static partial class GetSignInData
{
	[UsedImplicitly]
	public sealed record Query(string Name);
	
	private static async ValueTask<Ok<Result>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var user = await context.Users
			.Where(u => u.NormalizedUserName == request.Name.Normalize().ToUpperInvariant())
			.Select(u => new Result(u.Avatar, u.Title))
			.FirstOrDefaultAsync(cancellationToken);

		var data = user ?? new Result(Lorem.Picsum(200), string.Empty);

		return TypedResults.Ok(data);
	}
	
	public sealed record Result(string Avatar, string? Title);
}