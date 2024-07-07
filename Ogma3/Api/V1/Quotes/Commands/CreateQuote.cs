using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ogma3.Data;
using Ogma3.Data.Quotes;
using Ogma3.Infrastructure.Mediator.Bases;

namespace Ogma3.Api.V1.Quotes.Commands;

public static class CreateQuote
{
	public sealed record Command(string Body, string Author) : IRequest<ActionResult<QuoteDto>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(q => q.Body).NotEmpty();
			RuleFor(q => q.Author).NotEmpty();
		}
	}

	public class CreateQuoteHandler(ApplicationDbContext context, ILogger<CreateQuotesFromJson.CreateQuoteHandler> logger)
		: BaseHandler, IRequestHandler<Command, ActionResult<QuoteDto>>
	{
		public async ValueTask<ActionResult<QuoteDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			var (body, author) = request;
			var quote = new Quote
			{
				Body = body,
				Author = author,
			};
			context.Quotes.Add(quote);

			try
			{
				await context.SaveChangesAsync(cancellationToken);
			}
			catch (DbUpdateException ex)
			{
				logger.LogError("Creation error in {Src}: {Msg}", ex.Source, ex.Message);
				return ServerError("Database Creation Error");
			}
			
			logger.LogInformation("Redirecting to {Controller}.{Action}", nameof(QuotesController), nameof(QuotesController.GetQuote));

			return CreatedAtAction(
				nameof(QuotesController.GetQuote),
				nameof(QuotesController)[..^10],
				new { id = quote.Id },
				new QuoteDto(quote.Author, quote.Body)
			);
		}
	}
}