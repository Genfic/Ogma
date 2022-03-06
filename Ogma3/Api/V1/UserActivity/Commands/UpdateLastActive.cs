using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.UserActivity.Commands;

public static class UpdateLastActive
{
    public sealed record Command : IRequest<OkResult>;

    public class Handler : BaseHandler, IRequestHandler<Command, OkResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;

        public Handler(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _uid = userService.User?.GetNumericId();
        }

        public async Task<OkResult> Handle(Command request, CancellationToken cancellationToken)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ""AspNetUsers"" SET ""LastActive"" = {DateTime.Now.ToUniversalTime()} WHERE ""Id"" = {_uid}", 
                cancellationToken
            );
            return Ok();
        }
    }
}