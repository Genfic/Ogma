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
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class CreateFaq
{
	public sealed record Command(string Question, string Answer);

	public class CommandValidator : AbstractValidator<Command>
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
		var (question, answer) = request;

		var faq = new Faq
		{
			Question = question,
			Answer = answer,
			AnswerRendered = Markdown.ToHtml(answer, MarkdownPipelines.All),
		};
		context.Faqs.Add(faq);

		await context.SaveChangesAsync(cancellationToken);

		var dto = new FaqDto(faq.Question, faq.Answer, faq.AnswerRendered);

		return TypedResults.CreatedAtRoute(dto, nameof(GetSingleFaq), new { faq.Id });
	}
}