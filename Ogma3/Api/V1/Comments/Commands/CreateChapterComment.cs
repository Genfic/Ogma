using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Data.Infractions;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services;
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

    public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<CommentDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly CommentRedirector _redirector;
        private readonly NotificationsRepository _notificationsRepo;
        private readonly long? _uid;
            
        public Handler(ApplicationDbContext context, IMapper mapper, CommentRedirector redirector, NotificationsRepository notificationsRepo, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _redirector = redirector;
            _notificationsRepo = notificationsRepo;
            _uid = userService?.User?.GetNumericId();
        }

        public async Task<ActionResult<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (_uid is null) return Unauthorized();
                
            // Check if user is muted
            var isMuted = await _context.Infractions
                .Where(i => i.UserId == _uid && i.Type == InfractionType.Mute)
                .AnyAsync(cancellationToken);
            if (isMuted) return Unauthorized();

            var (body, threadId) = request;

            var thread = await _context.CommentThreads
                .Where(ct => ct.Id == threadId)
                .FirstOrDefaultAsync(cancellationToken);

            if (thread is null) return NotFound();
            if (thread.LockDate is not null) return Unauthorized();
                
            var comment = new Comment
            {
                AuthorId = (long)_uid,
                Body = body,
                CommentsThreadId = threadId
            };
                
            _context.Comments.Add(comment);
            thread.CommentsCount++;

            await _context.SaveChangesAsync(cancellationToken);

            if (thread.UserId != _uid)
            {
                // Create notification
                var subscribers = await _context.CommentsThreadSubscribers
                    .Where(cts => cts.CommentsThreadId == thread.Id)
                    .Select(cts => cts.OgmaUserId)
                    .ToListAsync(cancellationToken);

                var redirection = await _redirector.RedirectToComment(comment.Id);
                if (redirection is not null)
                {
                    await _notificationsRepo.Create(ENotificationEvent.WatchedThreadNewComment,
                        subscribers,
                        redirection.Url,
                        redirection.Params,
                        redirection.Fragment,
                        comment.Body.Truncate(50)
                    );
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            return CreatedAtAction(
                nameof(CommentsController.GetComment),
                nameof(CommentsController)[..^10],
                new { comment.Id },
                _mapper.Map<Comment, CommentDto>(comment)
            );
        }
    }
}