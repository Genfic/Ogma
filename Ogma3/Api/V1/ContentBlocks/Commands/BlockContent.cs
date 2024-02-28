using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ContentBlocks.Commands;

public static class BlockContent
{
	// ReSharper disable once UnusedTypeParameter
	public sealed record Command<T>(long ObjectId, string Reason) : IRequest<ActionResult> where T : BaseModel, IBlockableContent;

	// TODO: Apparently, Mediator doesn't like generic handlers, so... figure something else out
	public class Handler<T>(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command<T>, ActionResult>
		where T : BaseModel, IBlockableContent
	{
		public async ValueTask<ActionResult> Handle(Command<T> request, CancellationToken cancellationToken)
		{
			var (itemId, reason) = request;

			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();

			var cb = new ContentBlock
			{
				Reason = reason,
				IssuerId = uid
			};

			var res = await context.Set<T>()
				.Where(i => i.Id == itemId)
				.ExecuteUpdateAsync(i => i.SetProperty(p => p.ContentBlock, cb), cancellationToken);

			return res > 0 ? Ok() : NotFound();
		}
	}
}