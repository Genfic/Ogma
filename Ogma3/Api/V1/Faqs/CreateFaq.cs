using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
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
	public sealed record Command(string Question, string Answer);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(f => f.Question).NotEmpty();
			RuleFor(f => f.Answer).NotEmpty();
		}
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