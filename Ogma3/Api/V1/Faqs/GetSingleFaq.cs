using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Faqs;

namespace Ogma3.Api.V1.Faqs;

using ReturnType = Results<NotFound, Ok<FaqDto>>;

[Handler]
[MapGet("api/faqs/{faqId:long}")]
public static partial class GetSingleFaq
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetSingleFaq));

	[Validate]
	public sealed partial record Query(long FaqId) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> Handle(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var faq = await context.Faqs
			.Where(f => f.Id == request.FaqId)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return faq is null
			? TypedResults.NotFound()
			: TypedResults.Ok(faq);
	}
}