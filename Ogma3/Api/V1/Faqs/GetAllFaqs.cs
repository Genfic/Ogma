using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Faqs;

namespace Ogma3.Api.V1.Faqs;

using ReturnType = Ok<FaqDto[]>;

[Handler]
[MapGet("api/faqs")]
public static partial class GetAllFaqs
{
	public sealed record Query;

	private static async ValueTask<ReturnType> HandleAsync(
		Query _,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var faqs = await context.Faqs
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(faqs);
	}
}