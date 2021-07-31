using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1.Roles.Commands
{
    public static class DeleteRole
    {
        public sealed record Query(long Id) : IRequest<IActionResult>;

        public class Handler : IRequestHandler<Query, IActionResult>
        {
            private readonly RoleManager<OgmaRole> _roleManager;
            public Handler(RoleManager<OgmaRole> roleManager) => _roleManager = roleManager;

            public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var role = await _roleManager.FindByIdAsync(request.Id.ToString());

                if (role is null) return new NotFoundResult();

                await _roleManager.DeleteAsync(role);

                return new OkObjectResult(role.Id);
            }
        }
    }
}