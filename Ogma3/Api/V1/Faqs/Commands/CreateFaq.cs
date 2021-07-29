using System.Threading;
using System.Threading.Tasks;
using Markdig;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.Faqs;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Faqs.Commands
{
    public static class CreateFaq
    {
        public sealed record Query(string Question, string Answer) : IRequest<ActionResult<Faq>>;

        public class Handler : IRequestHandler<Query, ActionResult<Faq>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<ActionResult<Faq>> Handle(Query request, CancellationToken cancellationToken)
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

                return new CreatedAtActionResult(
                    nameof(FaqsController.GetFaq),
                    nameof(FaqsController)[..^10],
                    new { faq.Id },
                    faq
                );
            }
        }
    }
}