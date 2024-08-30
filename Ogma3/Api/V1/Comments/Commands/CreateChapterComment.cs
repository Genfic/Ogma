using AutoMapper;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Data.Infractions;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;
using Utils.Extensions;

namespace Ogma3.Api.V1.Comments.Commands;

public static class CreateChapterComment
{
	public sealed record Command(string Body, long ThreadId) : IRequest<ActionResult<CommentDto>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(c => c.Body)
				.MinimumLength(CTConfig.CComment.MinBodyLength)
				.MaximumLength(CTConfig.CComment.MaxBodyLength);
		}
	}

	public class Handler(ApplicationDbContext context, IMapper mapper, NotificationsRepository notificationsRepo, IUserService userService)
		: BaseHandler, IRequestHandler<Command, ActionResult<CommentDto>>
	{
		private readonly long? _uid = userService.User?.GetNumericId();

		public async ValueTask<ActionResult<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			// Check if user is muted
			var isMuted = await context.Infractions
				.Where(i => i.UserId == _uid && i.Type == InfractionType.Mute)
				.AnyAsync(cancellationToken);
			if (isMuted) return Unauthorized();

			var (body, threadId) = request;

			var thread = await context.CommentThreads
				.Where(ct => ct.Id == threadId)
				.FirstOrDefaultAsync(cancellationToken);

			if (thread is null) return NotFound();
			if (thread.LockDate is not null) return Unauthorized();

			var comment = new Comment
			{
				AuthorId = (long)_uid,
				Body = body,
				CommentsThreadId = threadId,
			};

			context.Comments.Add(comment);
			thread.CommentsCount++;

			await context.SaveChangesAsync(cancellationToken);

			if (thread.UserId != _uid)
			{
				await notificationsRepo.NotifyUsers(thread.Id, comment.Id, comment.Body.Truncate(50), cancellationToken);
			}

			return CreatedAtAction(
				nameof(CommentsController.GetComment),
				nameof(CommentsController)[..^10],
				new { comment.Id },
				mapper.Map<Comment, CommentDto>(comment)
			);
		}
	}
}