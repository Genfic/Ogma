using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments.Commands;

public static class UpdateComment
{
	public sealed record Command(string Body, long Id) : IRequest<ActionResult<CommentDto>>;

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(c => c.Body)
				.MinimumLength(CTConfig.CComment.MinBodyLength)
				.MaximumLength(CTConfig.CComment.MaxBodyLength);
		}
	}

	public class Handler(ApplicationDbContext context, IUserService userService)
		: BaseHandler, IRequestHandler<Command, ActionResult<CommentDto>>
	{
		public async ValueTask<ActionResult<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();

			var (body, commentId) = request;

			var comment = await context.Comments
				.Where(c => c.Id == commentId)
				.Where(c => c.AuthorId == uid)
				.FirstOrDefaultAsync(cancellationToken);

			if (comment is null) return NotFound();

			// Create revision
			context.CommentRevisions.Add(new CommentRevision
			{
				Body = comment.Body,
				ParentId = comment.Id,
			});

			// Edit the comment
			comment.Body = body;
			comment.LastEdit = DateTime.Now;
			comment.EditCount += 1;

			await context.SaveChangesAsync(cancellationToken);

			var dto = CommentDto.FromComment(comment, uid);
			dto.Owned = uid == comment.AuthorId;

			return dto;
		}
	}
}