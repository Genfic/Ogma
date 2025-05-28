using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ogma3.Data;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Faqs;

using ReturnType = CreatedAtRoute<FaqDto>;

[Handler]
[MapPost("api/faqs")]
[Authorize(AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateFaq
{
	[Validate]
	public sealed partial record Command : IValidationTarget<Command>
	{
		[NotEmpty]
		public required string Question { get; init; }
		[NotEmpty]
		public required string Answer { get; init; }
	}

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var faq = new Faq
		{
			Question = request.Question,
			Answer = request.Answer,
			AnswerRendered = Markdown.ToHtml(request.Answer, MarkdownPipelines.All),
		};
		context.Faqs.Add(faq);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.CreatedAtRoute(faq.ToDto(), nameof(GetSingleFaq), new GetSingleFaq.Query(faq.Id));
	}
}