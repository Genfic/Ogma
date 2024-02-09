using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments.Commands;

public static class DeleteComment
{
	public sealed record Command(long Id) : IRequest<ActionResult<long>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<long>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<long>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var comment = await _context.Comments
				.Where(c => c.Id == request.Id)
				.Include(c => c.Revisions)
				.FirstOrDefaultAsync(cancellationToken);

			if (comment is null) return NotFound();
			if (comment.AuthorId != _uid) return Unauthorized();

			// Wipe comment
			comment.DeletedBy = EDeletedBy.User;
			comment.DeletedByUserId = _uid;
			comment.Body = string.Empty;
			comment.LastEdit = null;
			comment.EditCount = 0;

			// Wipe revisions
			comment.Revisions.Clear();

			await _context.SaveChangesAsync(cancellationToken);

			return Ok(comment.Id);
		}
	}
}