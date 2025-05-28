using System.Security.Claims;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Services.UserService;

public sealed class UserService(IHttpContextAccessor? accessor) : IUserService
{
	public ClaimsPrincipal? User => accessor?.HttpContext?.User;
	public long? UserId => User?.GetNumericId();
}