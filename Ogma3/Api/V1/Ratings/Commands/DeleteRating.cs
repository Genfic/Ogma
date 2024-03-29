using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.FileUploader;

namespace Ogma3.Api.V1.Ratings.Commands;

public static class DeleteRating
{
	public sealed record Command(long RatingId) : IRequest<ActionResult<long>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<long>>
	{
		private readonly ApplicationDbContext _context;
		private readonly ImageUploader _uploader;

		public Handler(ApplicationDbContext context, ImageUploader uploader)
		{
			_context = context;
			_uploader = uploader;
		}

		public async ValueTask<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
		{
			var rating = await _context.Ratings
				.Where(r => r.Id == request.RatingId)
				.FirstOrDefaultAsync(cancellationToken);

			if (rating is null) return NotFound();

			_context.Ratings.Remove(rating);

			if (rating is { Icon: not null, IconId: not null })
			{
				await _uploader.Delete(rating.Icon, rating.IconId, cancellationToken);
			}

			await _context.SaveChangesAsync(cancellationToken);

			return Ok(rating.Id);
		}
	}
}