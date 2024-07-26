using FluentValidation;
using Markdig;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Faqs.Commands;

public static class UpdateFaq
{
	public sealed record Command(long Id, string Question, string Answer) : IRequest<ActionResult>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(f => f.Question).NotEmpty();
			RuleFor(f => f.Answer).NotEmpty();
		}
	}

	public class Handler(ApplicationDbContext context) : BaseHandler, IRequestHandler<Command, ActionResult>
	{
		public async ValueTask<ActionResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var (id, question, answer) = request;

			var rendered = Markdown.ToHtml(answer, MarkdownPipelines.All);

			var res = await context.Faqs
				.Where(f => f.Id == id)
				.ExecuteUpdateAsync(f => f
						.SetProperty(x => x.Question, question)
						.SetProperty(x => x.Answer, answer)
						.SetProperty(x => x.AnswerRendered, rendered),
					cancellationToken);

			await context.SaveChangesAsync(cancellationToken);

			return res > 0 ? Ok() : NotFound();
		}
	}
}