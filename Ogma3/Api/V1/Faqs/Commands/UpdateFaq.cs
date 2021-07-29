using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Markdig;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Api.V1.Faqs.Commands
{
    public static class UpdateFaq
    {
        public sealed record Query(long Id, string Question, string Answer) : IRequest<IActionResult>;

        public class Handler : IRequestHandler<Query, IActionResult>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var (id, question, answer) = request;
                
                var faq = await _context.Faqs
                    .Where(f => f.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (faq is null) return new NotFoundResult();

                faq.Question = question;
                faq.Answer = answer;
                faq.AnswerRendered = Markdown.ToHtml(answer, MarkdownPipelines.All);

                await _context.SaveChangesAsync(cancellationToken);

                return new OkResult();
            }
        }
    }
}