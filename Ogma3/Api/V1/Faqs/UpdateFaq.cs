using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Faqs;

using ReturnType = Results<NotFound, Ok>;

[Handler]
[MapPut("api/faqs")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateFaq
{
	public sealed record Command(long Id, string Question, string Answer);

	[UsedImplicitly]
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
		var rendered = Markdown.ToHtml(request.Answer, MarkdownPipelines.All);

		var res = await context.Faqs
			.Where(f => f.Id == request.Id)
			.ExecuteUpdateAsync(f => f
					.SetProperty(x => x.Question, request.Question)
					.SetProperty(x => x.Answer, request.Answer)
					.SetProperty(x => x.AnswerRendered, rendered),
				cancellationToken);

		return res > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}