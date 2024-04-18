using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ContentBlocks.Commands;

public static class BlockContent
{
	public sealed record Command(long ObjectId, string Reason, BlockableContentType Type) : IRequest<ActionResult>;

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command, ActionResult>
	{
		public async ValueTask<ActionResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var (itemId, reason, type) = request;

			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();

			var cb = new ContentBlock
			{
				Reason = reason,
				IssuerId = uid
			};
			
			IQueryable<BaseBlockableModel> query = type switch
			{
				BlockableContentType.Blogpost => context.Blogposts.AsQueryable(),
				BlockableContentType.Chapter => context.Chapters.AsQueryable(),
				BlockableContentType.Story => context.Stories.AsQueryable(),
				_ => throw new UnexpectedEnumValueException<BlockableContentType>(type)
			};

			var res = await query
				.Where(i => i.Id == itemId)
				.ExecuteUpdateAsync(i => i.SetProperty(p => p.ContentBlock, cb), cancellationToken);

			return res > 0 ? Ok() : NotFound();
		}
	}
	
	public enum BlockableContentType
	{
		Story,
		Chapter,
		Blogpost,
	}
}