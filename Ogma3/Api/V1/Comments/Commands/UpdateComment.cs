using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Comments.Commands;

public static class UpdateComment
{
	public sealed record Command(string Body, long Id) : IRequest<ActionResult<CommentDto>>;

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
		private readonly long? _uid;
		private readonly IMapper _mapper;

		public Handler(ApplicationDbContext context, IUserService userService, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var (body, commentId) = request;

			var comm = await _context.Comments
				.Where(c => c.Id == commentId)
				.Where(c => c.AuthorId == _uid)
				.FirstOrDefaultAsync(cancellationToken);

			if (comm is null) return NotFound();

			// Create revision
			_context.CommentRevisions.Add(new CommentRevision
			{
				Body = comm.Body,
				ParentId = comm.Id
			});

			// Edit the comment
			comm.Body = body;
			comm.LastEdit = DateTime.Now;
			comm.EditCount += 1;

			await _context.SaveChangesAsync(cancellationToken);

			var dto = _mapper.Map<Comment, CommentDto>(comm);
			dto.Owned = _uid == comm.AuthorId;

			return dto;
		}
	}
}