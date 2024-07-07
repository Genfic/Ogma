using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Markdig;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Faqs.Commands;

public static class CreateFaq
{
	public sealed record Command(string Question, string Answer) : IRequest<ActionResult<FaqDto>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(f => f.Question).NotEmpty();
			RuleFor(f => f.Answer).NotEmpty();
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<FaqDto>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async ValueTask<ActionResult<FaqDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			var (question, answer) = request;

			var faq = new Faq
			{
				Question = question,
				Answer = answer,
				AnswerRendered = Markdown.ToHtml(answer, MarkdownPipelines.All)
			};
			_context.Faqs.Add(faq);

			await _context.SaveChangesAsync(cancellationToken);

			return CreatedAtAction(
				nameof(FaqsController.GetFaq),
				nameof(FaqsController)[..^10],
				new { faq.Id },
				new FaqDto(faq.Question, faq.Answer, faq.AnswerRendered)
			);
		}
	}
}