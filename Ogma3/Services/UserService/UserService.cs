#nullable enable

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor? _accessor;

        public UserService(IHttpContextAccessor? accessor) {
            _accessor = accessor;
        }
        
        public ClaimsPrincipal? User => _accessor?.HttpContext?.User;
    }
}